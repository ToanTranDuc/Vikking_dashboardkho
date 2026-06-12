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
    TenMau, Min = "", Max = "", StatusChuyen = 0, isDecat = "", DotSX = "", Makho = "", MaKhoDen = "", MaPKLChuyen = "", isTonKho = "";
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

const swalWithBootstrapButtons = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-success',
        cancelButton: 'btn btn-danger'
    },
    buttonsStyling: false
})

const Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 1000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
})

//reset 
function reset() {
    handleNhapKho.html("Nhập kho")
    select_selected.html("Tất cả")
    handleFast.html("Nhập tất cả")
    handleByQuantity.css("display", "block")
    option.removeClass("active")
    typeAction = 2
    arrSave.length = 0
    refresh = false;
    search.val('')
    isSave = false;
}
//end

//ajax get data
function GetLenhSX(statuschuyen) {
    $.ajax({
        async: false,
        url: '/api/NhapKho/GetLenhSX?statuschuyen=' + statuschuyen + '&username=' + username + '&status=' + statusHidePKL,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {
                let inputValue = data[0].displayname; // Giả sử data[0].displayname là chuỗi HTML
                let textValue = $("<div>").html(inputValue).text();
                arrLenhSX = data
                Option_slc(data)
                input.val(textValue)
                MaLenh = data[0].MaLenh
                MaPKL = data[0].MaPKL
                MaHang = data[0].MaHang
                PO = data[0].PO
                POID = data[0].POID
                MaDVSX = data[0].MaDVSX
                MaDVSXHoanThanh = data[0].MaDVSXHoanThanh
                isDecat = data[0].isDecat;
                DotSX = data[0].DotSX;
                GetDS_KHDT(MaPKL, MaHang, POID, StatusChuyen, MaDVSX, isDecat, DotSX)
                GetDSKho(MaDVSXHoanThanh)
            }
            else {
                renderTbl_DS_KHDT('');
                renderDsThung('');
            }
            //MaPKL = data[0].MaPKL

        }
    });
}

function GetLenhSXLuuChuyen() {
    $.ajax({
        async: false,
        url: '/api/NhapKho/GetLenhSXLuuChuyen?username=' + username,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {

            if (data.length > 0) {

                let inputValue = data[0].displayname; // Giả sử data[0].displayname là chuỗi HTML
                let textValue = $("<div>").html(inputValue).text();
                arrLenhSX = data
                Option_slc(data)
                input.val(textValue)
                MaLenh = data[0].MaLenh
                MaPKL = data[0].MaPKL
                MaHang = data[0].MaHang
                PO = data[0].PO
                POID = data[0].POID
                MaDVSX = data[0].MaDVSX
                isDecat = data[0].isDecat;
                DotSX = data[0].DotSX;
                MaPKLChuyen = data[0].MaPKL_Chuyen
                GetDS_KHDTLuanChuyen(MaPKL, MaHang, POID, MaPKLChuyen, MaDVSX, isDecat, DotSX)
                GetDSKho(MaDVSX)
            }
            else {
                renderTbl_DS_KHDT('');
                renderDsThung('');
                input.val('')
                slc_lenhsx.html('')

            }
            //MaPKL = data[0].MaPKL

        }
    });
}

function GetDS_KHDT(maplk, mahang, po, statuschuyen, madvsx, isDecat, dotsx) {
    $.ajax({
        async: false,
        url: '/api/NhapKho/GetDS_KHDT?maplk=' + maplk + '&mahang=' + mahang + '&poid=' + po + '&statuschuyen=' + statuschuyen + '&madvsx=' + madvsx + '&isDecat=' + isDecat + '&dotsx=' + dotsx,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {
                if (refresh == false) {
                    PO = data[0].PO
                    ColorID = data[0].ColorID
                    DauSize = data[0].DauSize
                    TuThung = data[0].TuThungDenThung.split('->')[0]
                    DenThung = data[0].TuThungDenThung.split('->')[1]
                    MaPKL = data[0].MaPKL
                    //MaHang = data[0].MaHang
                    POID = data[0].POID
                    MaDVSX = data[0].MaDVSX
                    Dot = data[0].Dot
                    MaDH = data[0].MaDH
                    Size = data[0].Size
                    TenMau = data[0].TenMau
                    Min = data[0].Min == null ? FindMin(data[0].SttThung) : data[0].Min
                    Max = data[0].Max == null ? FindMax(data[0].SttThung) : data[0].Max
                    if (!isLuuChuyen) {
                        GetDSThung(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
                    }
                    else {
                        GetDSThungLuanChuyen(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
                        GetDSKho(data[0].MaKhoDen)
                    }
                }
                ArrDS_KHDT = data;
                renderTbl_DS_KHDT(data)
            }
            else {
                ArrDS_KHDT = []
                renderTbl_DS_KHDT([])
                $("#dsThung").html('')
            }
        }
    });
}

function GetDS_KHDTLuanChuyen(maplk, mahang, po, mapklchuyen, madvsx, isDecat, dotsx) {
    $.ajax({
        async: false,
        url: '/api/NhapKho/GetDS_KHDTLuanChuyen?maplk=' + maplk + '&mahang=' + mahang + '&poid=' + po + '&mapklchuyen=' + mapklchuyen + '&madvsx=' + madvsx + '&isDecat=' + isDecat + '&dotsx=' + dotsx,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {
                if (refresh == false) {
                    PO = data[0].PO
                    ColorID = data[0].ColorID
                    DauSize = data[0].DauSize
                    TuThung = data[0].TuThungDenThung.split('->')[0]
                    DenThung = data[0].TuThungDenThung.split('->')[1]
                    MaPKL = data[0].MaPKL
                    //MaHang = data[0].MaHang
                    POID = data[0].POID
                    MaDVSX = data[0].MaDVSX
                    Dot = data[0].Dot
                    MaDH = data[0].MaDH
                    Size = data[0].Size
                    TenMau = data[0].TenMau
                    isTonKho = data[0].IsTonKho;
                    Min = data[0].Min == null ? FindMin(data[0].SttThung) : data[0].Min
                    Max = data[0].Max == null ? FindMax(data[0].SttThung) : data[0].Max

                    if (!isLuuChuyen) {
                        GetDSThung(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
                    }
                    else {
                        GetDSThungLuanChuyen(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
                        GetDSKho(data[0].MaKhoDen)
                    }
                }
                ArrDS_KHDT = data;
                renderTbl_DS_KHDT(data)
            }
            else {
                ArrDS_KHDT = []
                renderTbl_DS_KHDT([])
                $("#dsThung").html('')

            }
        }
    });
}

//(string maLenh, string po, string colorid, string dausize, string tuthung, string denthung)
function GetDSThung(maplk, po, mahang, tuthung, denthung, madvsx, isDeCat, dotsx) {
    $.ajax({
        async: false,
        url: '/api/NhapKho/GetDSThung?maplk=' + maplk + '&poid=' + po + '&mahang=' + mahang + '&tuthung=' + tuthung +
            '&denthung=' + denthung + '&madvsx=' + madvsx + "&isDeCat=" + isDeCat + "&dotsx=" + dotsx,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            ArrDsDongThung = data
            renderDsThung(data)
        }
    });
}

function GetDSThungLuanChuyen(maplk, po, mahang, tuthung, denthung, madvsx, isDeCat, dotsx) {
    $.ajax({
        async: false,
        url: '/api/NhapKho/GetDSThungLuanChuyen?maplk=' + maplk + '&poid=' + po + '&mahang=' + mahang + '&tuthung=' + tuthung +
            '&denthung=' + denthung + '&madvsx=' + madvsx + "&isDeCat=" + isDeCat + "&dotsx=" + dotsx,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            ArrDsDongThung = data
            renderDsThung(data)
        }
    });
}

//get ds kho
function GetDSKho(madvsx) {
    $.ajax({
        async: false,
        url: '/api/NhapKho/GetKho?madvsx=' + madvsx,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {
                selectKho(data)
            }
            else {
                selectKho([])
            }
        }
    });
}
//end

//search
function handleSearch(key) {
    var result = ArrDS_KHDT.filter(function (item) {
        return removeSpace(item.Size).toLowerCase().includes(removeSpace(key.toLowerCase())) ||
            removeSpace(item.MaDH).toLowerCase().includes(removeSpace(key.toLowerCase())) ||
            removeSpace(item.TenMau).toLowerCase().includes(removeSpace(key.toLowerCase()));
    });
    if (result.length > 0) {
        rowIndex = 0
        if (refresh == false) {
            PO = result[0].PO
            ColorID = result[0].ColorID
            DauSize = result[0].DauSize
            TuThung = result[0].TuThungDenThung.split('->')[0]
            DenThung = result[0].TuThungDenThung.split('->')[1]
            MaPKL = result[0].MaPKL
            POID = result[0].POID
            MaDVSX = result[0].MaDVSX
            Dot = result[0].Dot
            MaDH = result[0].MaDH
            Size = result[0].Size
            TenMau = result[0].TenMau
            Min = result[0].Min == null ? FindMin(result[0].SttThung) : result[0].Min
            Max = result[0].Max == null ? FindMax(result[0].SttThung) : result[0].Max
            if (!isLuuChuyen) {
                GetDSThung(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
            }
            else {
                GetDSThungLuanChuyen(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
            }
        }
        renderTbl_DS_KHDT(result)
    }
    else {
        renderTbl_DS_KHDT(ArrDS_KHDT)
    }
}
//end ajax

//render data
function renderTbl_DS_KHDT(data) {
    //malenh, po, colorid, dausize, tuthung, denthung
    let html = ''
    if (data.length > 0) {
        data.map((x, index) => {
            let max = x.Max == null ? FindMax(x.SttThung) : x.Max
            let min = x.Min == null ? FindMin(x.SttThung) : x.Min
            html += `<tr class="${index == rowIndex ? 'active' : ''}" data-madh=${x.MaDH} data-po=${x.PO} data-poid=${x.POID}
                data-dausize="${x.DauSize}" data-tuthungdenthung="${x.TuThungDenThung}" data-size="${x.Size}" data-tenmau=${x.TenMau}
                data-mapkl=${x.MaPKL} data-madvsx=${x.MaDVSX} data-dot=${x.Dot} data-rowindex = ${index} data-min=${min} data-max=${max} data-id="thung${min}${max}"
                data-makhoden=${x.MaKhoDen} data-isTonKho=${x.IsTonKho}
            >`
            html += `
                <td>${x.MaDH}</td>
                <td>${x.MaHang}</td>
                <td>${x.PO}</td>
                <td>${x.TenDVSX}</td>
                <td>${x.Dot}</td>
                <td>${x.TenMau}</td>
                <td>${x.Size}</td>
                <td>${x.DauSize}</td>
                ${isLuuChuyen == 1 ?
                    `
                    <td>${x.TuKho}</td>
                    <td>${x.DenKho}</td>
                    `
                    :
                    ``
                }
                <td>${x.TuThungDenThung}</td>
                <td class="totalThung">${isLuuChuyen == 1 ? `<span>${x.SLNK_lC}</span>/${x.SLTH}` : `<span>${x.SLNK}</span>/${x.SLTH}`}</td>
                <td>${convertDatetime(x.NgayLapKH)}</td>
                
            `
            html += '</tr>'
        })

        tobdy_tbl_DS_KHDT.html(html);
    }
    tobdy_tbl_DS_KHDT.html(html);
}

function removeSpace(key) {
    // Sử dụng biểu thức chính quy để loại bỏ tất cả các khoảng trắng
    const keyReplace = key.replace(/\s/g, '');
    return keyReplace;
}

function Option_slc(data) {
    let html = "";
    if (data.length > 0) {
        data.map((x, index) => {
            html += `
        <tr class="${index == 0 ? "active" : ""}" data-isdecat=${x.isDecat}  data-mapkl="${x.MaPKL}" data-MaHang ="${x.MaHang}" 
            data-PO=${x.PO} data-displayname="${x.displayname}" data-madvsxht="${x.MaDVSXHoanThanh}" data-madvsx="${x.MaDVSX}" data-dotsx="${x.DotSX}" data-poid="${x.POID}" data-mapklchuyen="${x.MaPKL_Chuyen}" >
            <td>${x.displayname}</td>
        </tr>
        `
        })
    }
    $("#slc_lenhsx").html(html);
}

function selectKho(data) {
    let html = "";
    html = '<option value="">Chọn kho</option>'
    data.map((x, index) => {
        html += `
            <option value=${x.MaKho}>${x.TenKho}</option>
        `
    })
    slc_kho.html(html);
}

function renderDsThung(data) {
    //if(data.length == 0) return
    if (isLuuChuyen) {
        data = data.filter(item => {
            return item.StatusChuyen != 0 && item.IsNhapKho == 1;
        })
    }
    let html = "";
    if (data.length > 0) {
        if (isSave == true) {
            if (isLuuChuyen) {
                $("#tbl_DS_KHDT tbody tr.active .totalThung span").html(CountNhapKhoLuuChuyen(data))
            }
            else {
                $("#tbl_DS_KHDT tbody tr.active .totalThung span").html(CountNhapKho(data))
            }
        }
        let check = true;
        if (data[0].SttThung_decat != 0) {
            check = false;
        }
        if (isLuuChuyen == true) {
            data.forEach(x => {
                let stt = check == true ? x.SttThung_temp : x.SttThung_decat

                html += `
            <div class="thung-item ${typeAction == 1 ? (x.IsXacNhan == 1 ? "nhapkho" : "") : (x.StatusChuyen == 2 ? "nhapkho" : "")}" data-sttThung=${x.SttThung} id="thung${x.SttThung}">
                <p>${stt}</p>
                ${x.StatusChuyen == 2 ? img_chuaDongThung : img_daDongThung}
                ${typeAction == 1 ?
                        `${x.IsXacNhan == 0 ? `<input type="checkbox" />` : `<input type="checkbox" checked disabled />`}` :
                        `${x.StatusChuyen == 1 || x.StatusChuyen == 3 ? `<input type="checkbox" />` : `<input type="checkbox" checked disabled />`}`
                    }
            </div>
            `
            })
        }
        else {
            data.forEach(x => {
                let stt = check == true ? x.SttThung_temp : x.SttThung_decat

                html += `
            <div class="thung-item ${x.IsXacNhan == true ? "nhapkho" : ""}" data-sttThung=${x.SttThung} id="thung${x.SttThung}">
                <p>${stt}</p>
                ${x.IsNhapKho == false ? img_chuaDongThung : img_daDongThung}
                ${typeAction == 1 ? `${x.IsXacNhan == false ? `<input type="checkbox" />` : `<input type="checkbox" checked disabled />`}` : `${(x.IsXacNhan == false && x.IsNhapKho == false) ? `<input type="checkbox" />` : `<input type="checkbox" checked disabled />`}`}
            </div>
            `
            })
        }

        $("#dsThung").html(html);
        // Sau khi tất cả các phần tử đã được thêm vào DOM, gọi hàm tippy()
        data.forEach(x => {
            //0: size, 1: đầu size, 2: size: 3: số lượng
            let stt = check == true ? x.SttThung : x.SttThung_decat
            if (x.thung.includes('@')) {
                let _thung = x.thung.split('@');

                let html = ''
                let html1 = ''
                let txtXuatHang = ''
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

                    //html += `<div style="max-width: 200px;">Size: ${_size}<br>SL: ${_soluong}<br>Màu: ${_tenmau}<br>Đầu Size: ${_dausize}</div><br>`
                })
                //html += `${txtXuatHang = x.IsXacNhan == true ? "Đã xác nhận" : ""}`
                //html += `${isLuuChuyen ? x.TenKhoDen == null ? "" : x.TenKhoDen : x.TenKho == null ? "" : x.TenKho}`
                //html1 = `<div style="display: flex; gap: 10px; flex-wrap: wrap">${html}</div>`
                // Bước 1: Nhóm các đối tượng theo TenMau và DauSize
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
                    //content: `Size: ${x.Size}<br>SL: ${x.SoLuongSP}<br>Màu: ${x.TenMau}<br>Đầu Size: ${x.DauSize}`,
                    allowHTML: true,
                    content: table
                });
            }
            else {
                let _thung = x.thung.split('|');
                let _tenmau = _thung['0'];
                let _dausize = _thung['1'];
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
        //else {
        //    $("#dsThung").html("<h3>Thùng đã được nhập hết</h3>");
        //}
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
//

//on click
tobdy_tbl_DS_KHDT.on("click", "tr", function () {
    typeAction = 0
    reset()
    $("#tbl_DS_KHDT tbody tr").removeClass('active')
    $(this).addClass('active')
    MaDH = $(this).data("madh")
    PO = $(this).data("po")
    POID = $(this).data("poid")
    ColorID = $(this).data("colorid")
    DauSize = $(this).data("dausize")
    TuThung = $(this).data("tuthungdenthung").split('->')[0]
    DenThung = $(this).data("tuthungdenthung").split('->')[1]
    Min = $(this).data("min")
    Max = $(this).data("max")
    MaPKL = $(this).data("mapkl")
    MaDVSX = $(this).data("madvsx")
    Dot = $(this).data("dot")
    Size = $(this).data("size")
    TenMau = $(this).data("tenmau")
    rowIndex = $(this).data("rowindex")
    isTonKho = $(this).data("istonkho")
    if (!isLuuChuyen) {
        GetDSThung(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
    }
    else {
        GetDSThungLuanChuyen(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
    }
    //if (isLuuChuyen == 1) {
    //    MaKhoDen = $(this).data("makhoden")
    //    if (MaKhoDen != MaDVSX) {
    //        GetDSKho(MaKhoDen)
    //    }
    //}
})

//slc_lenhsx.on("click", "tr", function () {
//    MaLenh = $(this).data("malenh")
//    GetDS_KHDT(MaLenh)
//})


$("body #dsThung").on("click", ".thung-item input", function () {
    if (typeAction != 1 && Makho.length == 0) {
        Toast.fire({
            icon: 'warning',
            title: 'Vui lòng chọn kho'
        })
    }
    // Tìm phần tử cha gần nhất có class .thung-item
    let thungItem = $(this).closest('.thung-item');
    let SttThung = thungItem.attr('data-sttThung');
    // Lấy giá trị của thuộc tính checked
    var isChecked = $(this).prop('checked');
    // Kiểm tra nếu ô input đã được checked
    if (isChecked) {
        let newObj = { ...objSave }
        newObj.SttThung = SttThung
        newObj.MaDH = MaDH
        newObj.MaPKL = MaPKL
        newObj.MaDVSX = MaDVSX
        newObj.POID = POID
        if (typeAction == 1) {
            newObj.StatusChuyen = isLuuChuyen == true ? "1" : "0"
        }
        else {
            newObj.StatusChuyen = isLuuChuyen == true ? "2" : "0"
        }
        newObj.Makho = isLuuChuyen == false ? Makho : ""
        //newObj.MaKhoDen = isLuuChuyen == true ? Makho : ""
        arrSave.push(newObj)
    } else {
        arrSave = arrSave.filter(function (item) {
            return item.SttThung !== SttThung;
        });
    }
})

//handleNhapKho
handleNhapKho.on("click", function () {
    if (typeAction == 1) {
        // Cập nhật tất cả các phần tử trong mảng arrSave cho trường NgayLapKH
        const updatedArrSave = arrSave.map(item => ({
            ...item, // Giữ nguyên các trường dữ liệu khác nếu có
            NgayNhapKho: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
            IsNhapKho: isLuuChuyen == true ? "1" : "0",
            StatusChuyen: isLuuChuyen == true ? "1" : "0"
        }));
        if (updatedArrSave.length > 0) {
            UpdateStatus(updatedArrSave)
            handleByVitri.html("Nhập theo vị trí")
        }
        else {
            Toast.fire({
                icon: 'error',
                title: 'Chưa chọn thùng để hủy'
            })
        }
    }
    else {
        // Cập nhật tất cả các phần tử trong mảng arrSave cho trường NgayLapKH
        const updatedArrSave = arrSave.map(item => ({
            ...item, // Giữ nguyên các trường dữ liệu khác nếu có
            NgayNhapKho: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
            IsNhapKho: 1,
            StatusChuyen: isLuuChuyen == true ? "2" : "0"
        }));
        if (updatedArrSave.length > 0) {
            UpdateStatus(updatedArrSave)
        }
        else {
            Toast.fire({
                icon: 'error',
                title: 'Chưa chọn thùng để nhập'
            })
        }
    }
})

$("#btn-accept").on("click", function () {
    let arrFilter = []
    arrFilter = ArrDsDongThung.filter(function (item) {
        return item.IsNhapKho == 0;
    });
    console.log(arrFilter)
})

handleFast.on("click", function () {
    let arrFilter = []
    if (typeAction == 1) {
        //arrFilter = ArrDsDongThung.filter(function (item) {
        //    return item.IsNhapKho == 1 && item.IsXacNhan == 0;
        //});
        if (isLuuChuyen) {
            arrFilter = ArrDsDongThung.filter(function (item) {
                return item.IsNhapKho == 1 && item.IsXacNhan == 0;
            });
        }
        else {
            arrFilter = ArrDsDongThung.filter(function (item) {
                return item.IsXacNhan == false;
            });
        }
        if (arrFilter.length == 0) {
            Toast.fire({
                icon: 'warning',
                title: 'Không có thùng để hủy'
            })
        }
        else {
            swalWithBootstrapButtons.fire({
                title: "Hủy nhập toàn bộ thùng!",
                text: "Cảnh báo!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Đồng ý",
                cancelButtonText: "Hủy",
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    let newArray = arrFilter.map(({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayNhapKho, NgayNhapKho_TC, IsNhapKho, StatusChuyen }) => ({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayNhapKho, NgayNhapKho_TC, IsNhapKho, StatusChuyen }));
                    if (typeAction == 1) {
                        const updatedArrSave = newArray.map(item => ({
                            ...item, // Giữ nguyên các trường dữ liệu khác nếu có
                            SttThung: item.SttThung,
                            MaDH: MaDH,
                            MaPKL: MaPKL,
                            MaDVSX: MaDVSX,
                            POID: POID,
                            NgayNhapKho: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
                            NgayNhapKho_TC: formattedDateTime,
                            IsNhapKho: 0,
                            Makho: null,
                            StatusChuyen: isLuuChuyen == true ? "1" : "0"
                        }));
                        UpdateStatus(updatedArrSave)
                    }
                    else {
                        const updatedArrSave = newArray.map(item => ({
                            ...item, // Giữ nguyên các trường dữ liệu khác nếu có
                            SttThung: item.SttThung,
                            MaDH: MaDH,
                            MaPKL: MaPKL,
                            MaDVSX: MaDVSX,
                            POID: POID,
                            NgayNhapKho: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
                            NgayNhapKho_TC: formattedDateTime,
                            IsNhapKho: 0,
                            Makho: null,
                            StatusChuyen: isLuuChuyen == true ? "2" : "0"
                        }));
                        UpdateStatus(updatedArrSave)
                    }
                }
            });
        }
    }
    else {
        if (isLuuChuyen) {
            arrFilter = ArrDsDongThung.filter(function (item) {
                return item.StatusChuyen == 1;
            });
        }
        else {
            arrFilter = ArrDsDongThung.filter(function (item) {
                return item.IsNhapKho == 0;
            });
        }
        if (arrFilter.length == 0) {
            Toast.fire({
                icon: 'warning',
                title: 'Thùng đã được nhập hết'
            })
        }
        else {
            Swal.fire({
                title: "Nhập toàn bộ thùng!",
                text: "Cảnh báo!",
                html: `
                        <input id="swal-input3" class="swal2-input" type="date" value="${new Date().toISOString().split('T')[0]}" >
                    `,
                focusConfirm: false,
                preConfirm: () => {
                    return document.getElementById('swal-input3').value
                },
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Đồng ý",
                cancelButtonText: "Hủy",
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    let newArray = arrFilter.map(({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayNhapKho, NgayNhapKho_TC, IsNhapKho, StatusChuyen }) => ({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayNhapKho, NgayNhapKho_TC, IsNhapKho, StatusChuyen }));
                    const updatedArrSave = newArray.map(item => ({
                        ...item, // Giữ nguyên các trường dữ liệu khác nếu có
                        SttThung: item.SttThung,
                        MaDH: MaDH,
                        MaPKL: MaPKL,
                        MaDVSX: MaDVSX,
                        POID: POID,
                        NgayNhapKho: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
                        NgayNhapKho_TC: $("#swal-input3").val(),
                        IsNhapKho: 1,
                        StatusChuyen: 2,
                        Makho: Makho
                    }));
                    UpdateStatus(updatedArrSave)
                }
            });
        }
    }
})

$("#handleByQuantity").on("click", function () {
    let arrFilter = []
    if (isLuuChuyen) {
        arrFilter = ArrDsDongThung.filter(function (item) {
            return item.StatusChuyen == 1;
        });
    }
    else {
        arrFilter = ArrDsDongThung.filter(function (item) {
            return item.IsNhapKho == 0;
        });
    }
    if (arrFilter.length == 0) {
        Toast.fire({
            icon: 'warning',
            title: 'Thùng đã được nhập hết'
        })
    }
    else {
        Swal.fire({
            title: "Nhập số lượng thùng",
            html:
                '<input id="swal-input1" style="text-align: center" class="swal2-input" type="number" placeholder="Số lượng thùng">' +
                `<input id="swal-input2" class="swal2-input" type="date" value="${new Date().toISOString().split('T')[0]}" >`,
            showCancelButton: true,
            cancelButtonText: "Hủy",
            confirmButtonText: "Xác nhận",
            showLoaderOnConfirm: true,
            allowOutsideClick: false,
            preConfirm: () => {
                return [
                    document.getElementById('swal-input1').value,
                    document.getElementById('swal-input2').value
                ];
            }
        }).then((result) => {
            if (result.isConfirmed) {
                const [quantity, date] = result.value;
                let SoLuong = quantity;
                if (SoLuong > 0) {
                    let newArray = arrFilter.slice(0, SoLuong).map(({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayNhapKho, NgayNhapKho_TC, IsNhapKho, StatusChuyen }) => ({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayNhapKho, NgayNhapKho_TC, IsNhapKho, StatusChuyen }));
                    const updatedArrSave = newArray.map(item => ({
                        ...item, // Giữ nguyên các trường dữ liệu khác nếu có
                        SttThung: item.SttThung,
                        MaDH: MaDH,
                        MaPKL: MaPKL,
                        MaDVSX: MaDVSX,
                        POID: POID,
                        NgayNhapKho: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
                        NgayNhapKho_TC: date,
                        IsNhapKho: 1,
                        StatusChuyen: 2,
                        Makho: Makho
                    }));
                    UpdateStatus(updatedArrSave)
                }
                else {
                    Toast.fire({
                        icon: 'warning',
                        title: 'Số lượng thùng không hợp lệ'
                    })
                }
            }
        });
    }
})
//end
//Nhập kho theo vị trí
$("#btnViTriXacNhan").on("click", function () {
    let ViTriBD = $("#ViTriBD").val();
    let ViTriKT = $("#ViTriKT").val();
    let arrFilter = []
    if (Number(ViTriKT) < Number(ViTriBD)) {
        Toast.fire({
            icon: 'warning',
            title: 'Vị trí kết thúc không hợp lệ'
        })
    }
    else {
        //Hủy kho theo vị trí
        if (typeAction == 1) {
            if (isLuuChuyen) {
                arrFilter = ArrDsDongThung.filter(function (item) {
                    return item.IsNhapKho == 1 && item.IsXacNhan == 0;
                });
            }
            else {
                arrFilter = ArrDsDongThung.filter(function (item) {
                    return item.IsXacNhan == false;
                });
            }
            if (arrFilter.length == 0) {
                Toast.fire({
                    icon: 'warning',
                    title: 'Không có thùng để hủy'
                })
            }
            else {
                // const arrFilter1 = arrFilter.filter(item => item.SttThung_temp >= ViTriBD && item.SttThung_temp <= ViTriKT)

                let arrFilter1 = []; // = arrFilter.filter(item => item.SttThung_temp >= ViTriBD && item.SttThung_temp <= ViTriKT)

                var valueCheckDecat = arrFilter[0].SttThung_decat;
                if (valueCheckDecat != "0")
                    arrFilter1 = arrFilter.filter(item => item.SttThung_decat >= ViTriBD && item.SttThung_decat <= ViTriKT)
                else
                    arrFilter1 = arrFilter.filter(item => item.SttThung_temp >= ViTriBD && item.SttThung_temp <= ViTriKT)

                if (arrFilter1.length > 0) {
                    let newArray = arrFilter1.map(({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayNhapKho, NgayNhapKho_TC, IsNhapKho, StatusChuyen }) => ({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayNhapKho, NgayNhapKho_TC, IsNhapKho, StatusChuyen }));
                    const updatedArrSave = newArray.map(item => ({
                        ...item, // Giữ nguyên các trường dữ liệu khác nếu có
                        SttThung: item.SttThung,
                        MaDH: MaDH,
                        MaPKL: MaPKL,
                        MaDVSX: MaDVSX,
                        POID: POID,
                        NgayNhapKho: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
                        NgayNhapKho_TC: $("#ngayTheoViTri").val(),
                        IsNhapKho: 0,
                        Makho: null,
                        StatusChuyen: isLuuChuyen == true ? "1" : "0"
                    }));
                    UpdateStatus(updatedArrSave)
                }
                else {
                    Toast.fire({
                        icon: 'warning',
                        title: 'Vị trí thùng sai.'
                    })
                }
            }
        }
        //Nhập kho theo vị trí
        else {
            if (isLuuChuyen) {
                arrFilter = ArrDsDongThung.filter(function (item) {
                    return item.StatusChuyen == 1;
                });
            }
            else {
                arrFilter = ArrDsDongThung.filter(function (item) {
                    return item.IsNhapKho == 0;
                });
            }
            if (arrFilter.length == 0) {
                Toast.fire({
                    icon: 'warning',
                    title: 'Thùng đã được nhập hết'
                })
            }
            else {
                var valueCheckDecat = ArrDsDongThung[0].SttThung_decat;
                if (valueCheckDecat != "0")
                    arrFilter = ArrDsDongThung.filter(item => item.SttThung_decat >= ViTriBD && item.SttThung_decat <= ViTriKT)
                else
                    arrFilter = ArrDsDongThung.filter(item => item.SttThung_temp >= ViTriBD && item.SttThung_temp <= ViTriKT)
                if (arrFilter.length > 0) {
                    let newArray = arrFilter.map(({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayNhapKho, NgayNhapKho_TC, IsNhapKho, StatusChuyen }) => ({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayNhapKho, NgayNhapKho_TC, IsNhapKho, StatusChuyen }));
                    const updatedArrSave = newArray.map(item => ({
                        ...item, // Giữ nguyên các trường dữ liệu khác nếu có
                        SttThung: item.SttThung,
                        MaDH: MaDH,
                        MaPKL: MaPKL,
                        MaDVSX: MaDVSX,
                        POID: POID,
                        NgayNhapKho: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
                        NgayNhapKho_TC: $("#ngayTheoViTri").val(),
                        IsNhapKho: 1,
                        StatusChuyen: 2,
                        Makho: Makho
                    }));
                    UpdateStatus(updatedArrSave)
                }
                else {
                    Toast.fire({
                        icon: 'warning',
                        title: 'Vị trí thùng sai.'
                    })
                }
            }
        }
    }
    $("#ViTriBD").val("");
    $("#ViTriKT").val("");
})
//end
//

//convert
function convertDatetime(datetime) {
    const datePart = datetime.split('T');
    const [year, month, day] = datePart[0].split('-');
    const formattedDate = `${day}-${month}-${year}`;
    return formattedDate;
}
//end convert

//handle post
function UpdateStatus(arr) {
    //alert(isLuuChuyen)
    if (typeAction != 1 && Makho.length == '0') {
        Toast.fire({
            icon: 'error',
            title: 'Bạn chưa chọn kho để lưu!'
        })
    }
    else {
        let dataSave = arr;
        let url = isLuuChuyen == 0 ? '/api/NhapKho/PostNhapKho' : '/api/NhapKho/PostNhapKhoLuuChuyen?isTonKho=' + isTonKho
        if (typeAction != 1) {
            dataSave = arr.map(item => ({
                ...item,
                Makho: Makho
            }));
        }
        else {
            dataSave = arr.map(item => ({
                ...item,
                Makho: null
            }));
        }

        return $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            data: JSON.stringify(dataSave),
            contentType: 'application/json',
            success: function (data) {
                isSave = true;
                if (typeAction == 1) {
                    Toast.fire({
                        icon: 'success',
                        title: 'Hủy nhập kho thành công'
                    })
                }
                else {
                    Toast.fire({
                        icon: 'success',
                        title: 'Nhập kho thành công'
                    })
                }
                typeAction = 2
                if (!isLuuChuyen) {
                    GetDSThung(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
                }
                else {
                    GetDSThungLuanChuyen(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
                }
                reset()
                console.log("update success")
            }
        });
    }
};
//end

//check hide
function checkHide() {
    if (isHide) {
        btn_hide.html("Hiện")
        btn_hide.addClass("active")
    }
    else {
        btn_hide.html("Ẩn")
        btn_hide.removeClass("active")
    }

    if (btn_hide.hasClass("active")) {
        $(".dongthung__right").css("max-height", "calc(100vh - 180px)");
        $(".PSM").css("max-height", "calc(100vh - 180px)");
    }
}
//

$(document).ready(function () {
    GetLenhSX(StatusChuyen);

    search.on("input", function () {
        clearTimeout(timeoutId);
        timeoutId = setTimeout(() => {
            handleSearch($(this).val())
        }, 1000);
    })

    $(".toggle_input").on("click", function () {
        $(".box_toggle").slideToggle("slow");
    })

    $("#slc_lenhsx").on("click", "tr", function (event) {
        $("#slc_lenhsx tr").removeClass("active")
        $(".box_toggle").css("display", "none");
        $(this).addClass("active")
        rowIndex = 0
        MaLenh = $(this).data("malenh")
        MaPKL = $(this).data("mapkl")
        MaDVSX = $(this).data("madvsx")
        MaDVSXHoanThanh = $(this).data("madvsxht")
        MaHang = $(this).data("mahang")
        PO = $(this).data("po")
        POID = $(this).data("poid")
        DotSX = $(this).data('dotsx')
        isDecat = $(this).data("isdecat")
        if (!isLuuChuyen) {
            GetDS_KHDT(MaPKL, MaHang, POID, StatusChuyen, MaDVSX, isDecat, DotSX)
            GetDSKho(MaDVSXHoanThanh)
        }
        else {
            MaPKLChuyen = $(this).data("mapklchuyen")
            GetDS_KHDTLuanChuyen(MaPKL, MaHang, POID, MaPKLChuyen, MaDVSX, isDecat, DotSX)
        }

        displayname = $(this).attr("data-displayname");

        let inputValue = displayname; // Giả sử data[0].displayname là chuỗi HTML
        let textValue = $("<div>").html(inputValue).text(); // Chuyển đổi từ HTML sang văn bản thô
        input.val(textValue);
        Makho = ""
        reset()

        isHide = false;
        tobdy_tbl_DS_KHDT.show()
        checkHide()
    });

    slc_kho.on("change", function () {
        Makho = $(this).val()
    })

    customSelect.on("click", function () {
        if (customSelect.hasClass('open')) {
            customSelect.removeClass('open')
        }
        else {
            customSelect.addClass('open')
        }
    })

    btn_luuchuyen.on("click", function () {
        rowIndex = 0
        isLuuChuyen = !isLuuChuyen;
        Makho = ""
        if (isLuuChuyen) {
            StatusChuyen = 1
        }
        else {
            StatusChuyen = 0
        }
        if (!isLuuChuyen) {
            GetLenhSX(StatusChuyen)
            GetDS_KHDT(MaPKL, MaHang, POID, StatusChuyen, MaDVSX, isDecat, DotSX)
        }
        else {
            GetLenhSXLuuChuyen()
            //GetDS_KHDTLuanChuyen(MaPKL, MaHang, POID, MaPKLChuyen, MaDVSX, isDecat, DotSX)
        }


        $("#tbl_DS_KHDT tr th.diadiem").toggleClass("hide")
    })

    option.on("click", function () {
        let action = $(this).data("action")
        typeAction = action
        option.removeClass("active")
        $(this).addClass("active")
        console.log(ArrDsDongThung, "ArrDsDongThung")
        let arrFilter = []
        if (action == "1") {
            $("#ngayTheoViTri").css("display", "none")
        }
        else {
            $("#ngayTheoViTri").css("display", "inline-block")
        }

        if (isLuuChuyen == true) {
            if (action == "0") {
                arrFilter = ArrDsDongThung.filter(function (item) {
                    return item.StatusChuyen == 1;
                });
                select_selected.html("Chưa nhập")
                handleNhapKho.html("Nhập kho")
                handleFast.html("Nhập tất cả")
                handleByVitri.html("Nhập theo vị trí")
                handleByQuantity.css("display", "block")
            }
            else if (action == "1") {
                arrFilter = ArrDsDongThung.filter(function (item) {
                    return item.StatusChuyen == 2;
                });
                select_selected.html("Đã nhập")
                handleNhapKho.html("Hủy nhập")
                handleFast.html("Hủy nhập tất cả")
                handleByVitri.html("Hủy nhập theo vị trí")
                handleByQuantity.css("display", "none")
            }
            else {
                arrFilter = ArrDsDongThung;
                select_selected.html("Tất cả")
                handleNhapKho.html("Nhập kho")
                handleFast.html("Nhập tất cả")
                handleByVitri.html("Nhập theo vị trí")
                handleByQuantity.css("display", "block")
            }
        }
        else {
            if (action == "0") {
                arrFilter = ArrDsDongThung.filter(function (item) {
                    return item.IsNhapKho == 0;
                });
                select_selected.html("Chưa nhập")
                handleNhapKho.html("Nhập kho")
                handleFast.html("Nhập tất cả")
                handleByVitri.html("Nhập theo vị trí")
                handleByQuantity.css("display", "block")
            }
            else if (action == "1") {
                arrFilter = ArrDsDongThung.filter(function (item) {
                    return item.IsNhapKho == 1;
                });
                select_selected.html("Đã nhập")
                handleNhapKho.html("Hủy nhập")
                handleFast.html("Hủy nhập tất cả")
                handleByVitri.html("Hủy nhập theo vị trí")
                handleByQuantity.css("display", "none")
            }
            else {
                arrFilter = ArrDsDongThung;
                select_selected.html("Tất cả")
                handleNhapKho.html("Nhập kho")
                handleFast.html("Nhập tất cả")
                handleByVitri.html("Nhập theo vị trí")
                handleByQuantity.css("display", "block")
            }
        }
        renderDsThung(arrFilter)
    })

    //search
    $("#search_mahang").on("keyup", function (e) {
        clearTimeout(timeoutId);
        timeoutId = setTimeout(() => {
            let keyup = $(this).val().toLowerCase();
            keyup_search_mahang = keyup;
            //let arrLsxFilter = []
            //let arrLSX = arrLsxFilter.length > 0 ? arrLsxFilter : arrLenhSX;
            if (keyup_search_mahang.length == 0) {
                if (keyup_search_po.length == 0) {
                    arrLsxFilter = arrLenhSX
                }
                else {
                    arrLsxFilter = arrLenhSX.filter(function (item) {
                        return item.PO.toLowerCase().includes(keyup_search_po)
                    });
                }
            }
            else {
                if (keyup_search_po.length == 0) {
                    arrLsxFilter = arrLenhSX.filter(function (item) {
                        return item.MaHang.toLowerCase().includes(keyup)
                    });
                }
                else {
                    arrLsxFilter = arrLenhSX.filter(function (item) {
                        return item.MaHang.toLowerCase().includes(keyup) && item.PO.toLowerCase().includes(keyup_search_po)
                    });
                }
            }
            if (arrLsxFilter.length == 0) {
                Option_slc([])
            }
            else {
                Option_slc(arrLsxFilter)
            }
        }, 1000);
    })

    $("#search_po").on("keyup", function (e) {
        clearTimeout(timeoutId);
        timeoutId = setTimeout(() => {
            let keyup = $(this).val().toLowerCase();
            keyup_search_po = keyup;
            //let arrLsxFilter = []
            //let arrLSX = arrLsxFilter.length > 0 ? arrLsxFilter : arrLenhSX;
            if (keyup_search_po.length == 0) {
                if (keyup_search_mahang.length == 0) {
                    arrLsxFilter = arrLenhSX.filter(function (item) {
                        return item.MaHang.toLowerCase().includes(keyup_search_mahang)
                    });
                }
                else {
                    arrLsxFilter = arrLenhSX;
                }
            }
            else {
                if (keyup_search_mahang.length == 0) {
                    arrLsxFilter = arrLenhSX.filter(function (item) {
                        return item.PO.toLowerCase().includes(keyup)
                    });
                }
                else {
                    arrLsxFilter = arrLenhSX.filter(function (item) {
                        return item.PO.toLowerCase().includes(keyup) && item.MaHang.toLowerCase().includes(keyup_search_mahang)
                    });
                }
            }
            if (arrLsxFilter.length == 0) {
                Option_slc([])
            }
            else {
                Option_slc(arrLsxFilter)
            }
        }, 1000);
    })
    //end

    //refresh
    btn_refresh.on("click", function () {
        refresh = true;

        if (!isLuuChuyen) {
            GetDS_KHDT(MaPKL, MaHang, POID, StatusChuyen, MaDVSX, isDecat, DotSX)
            GetDSThung(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
        }
        else {
            GetDS_KHDTLuanChuyen(MaPKL, MaHang, POID, MaPKLChuyen, MaDVSX, isDecat, DotSX)
            GetDSThungLuanChuyen(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
        }
        arrSave.length = 0;
        search.val('')
    })
    //end

    //hide tbl
    btn_hide.on("click", function () {
        isHide = !isHide;
        tobdy_tbl_DS_KHDT.toggle()
        checkHide()
    })
    //end
    //remove key search
    $("#remove-search").on("click", function () {
        search.val('')
        handleSearch('')
    })
    //end

    //Hide PKL
    $("#hidePKL").on("click", function () {
        let checked = $(this).prop('checked');
        $("#search_mahang").val("")
        $("#search_po").val("")
        if (checked) {
            statusHidePKL = 1;
            GetLenhSX(StatusChuyen)
        }
        else {
            statusHidePKL = 0;
            GetLenhSX(StatusChuyen)
        }
    })
    //end
})

//end