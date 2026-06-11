//coding by Dat
//start

const input = $(".toggle_input input");
const img_chuaDongThung = `<svg width="20px" height="20px" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 512"><!--! Font Awesome Pro 6.2.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2022 Fonticons, Inc. --><path fill="#b82a2aa8" d="M58.9 42.1c3-6.1 9.6-9.6 16.3-8.7L320 64 564.8 33.4c6.7-.8 13.3 2.7 16.3 8.7l41.7 83.4c9 17.9-.6 39.6-19.8 45.1L439.6 217.3c-13.9 4-28.8-1.9-36.2-14.3L320 64 236.6 203c-7.4 12.4-22.3 18.3-36.2 14.3L37.1 170.6c-19.3-5.5-28.8-27.2-19.8-45.1L58.9 42.1zM321.1 128l54.9 91.4c14.9 24.8 44.6 36.6 72.5 28.6L576 211.6v167c0 22-15 41.2-36.4 46.6l-204.1 51c-10.2 2.6-20.9 2.6-31 0l-204.1-51C79 419.7 64 400.5 64 378.5v-167L191.6 248c27.8 8 57.6-3.8 72.5-28.6L318.9 128h2.2z"/></svg>`
const img_daDongThung = `<svg width="20px" height="20px" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><!--! Font Awesome Pro 6.2.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2022 Fonticons, Inc. --><path fill="#aabd78" d="M50.7 58.5L0 160H208V32H93.7C75.5 32 58.9 42.3 50.7 58.5zM240 160H448L397.3 58.5C389.1 42.3 372.5 32 354.3 32H240V160zm208 32H0V416c0 35.3 28.7 64 64 64H384c35.3 0 64-28.7 64-64V192z"/></svg>`
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
let MaLenh = "", PO = "", ColorID = "", DauSize = "", TuThung = "", DenThung = "", MaHang = "", MaPKL = "", MaDVSX = "",
    Dot = "", MaDH = "", POID = "", Size = "", TenMau, Min = "", Max = "", StatusChuyen = 0, isDecat = "", DotSX = "";
let keyup_search_po = "", keyup_search_mahang = ""
let statusHidePKL = 0;
let objSave = {
    SttThung: "",
    MaDH: "",
    MaPKL: "",
    MaDVSX: "",
    POID: "",
    NgayDongThung: "",
    IsDongThung: "",
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
    handleDongThung.html("Đóng thùng")
    select_selected.html("Tất cả")
    handleFast.html("Đóng tất cả")
    handleByVitri.html("Đóng theo vị trí")
    handleByQuantity.css("display", "block")
    option.removeClass("active")
    typeAction = 2
    arrSave.length = 0
    refresh = false;
    search.val('')
    StatusChuyen = 0
    isSave = false;
}
//end

//ajax get data
function GetLenhSX(statuschuyen) {
    $.ajax({
        async: false,
        url: '/api/DongThung/GetLenhSX?statuschuyen=' + statuschuyen + '&username=' + username + '&status=' + statusHidePKL,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
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
            //MaPKL = data[0].MaPKL
            GetDS_KHDT(MaPKL, MaHang, POID, StatusChuyen, MaDVSX, isDecat, DotSX)
        }
    });
}

function GetDS_KHDT(maplk, mahang, poid, statuschuyen, madvsx, isDecat, dotsx) {
    $.ajax({
        async: false,
        url: '/api/DongThung/GetDS_KHDT?maplk=' + maplk + '&mahang=' + mahang + '&poid=' + poid + '&statuschuyen=' + statuschuyen + '&madvsx=' + madvsx + '&isDecat=' + isDecat + '&dotsx=' + dotsx,
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
                    GetDSThung(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
                }
                ArrDS_KHDT = data;
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
function GetDSThung(maplk, poid, mahang, tuthung, denthung, madvsx, isDeCat, dotsx) {
    $.ajax({
        async: false,
        url: '/api/DongThung/GetDSThung?maplk=' + maplk + '&poid=' + poid + '&mahang=' + mahang + '&tuthung=' + tuthung + '&denthung=' + denthung + '&madvsx=' + madvsx + '&isDeCat=' + isDeCat + '&dotsx=' + dotsx,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            ArrDsDongThung = data
            renderDsThung(data)
        }
    });
}


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
            //MaHang = result[0].MaHang
            POID = result[0].POID
            MaDVSX = result[0].MaDVSX
            Dot = result[0].Dot
            MaDH = result[0].MaDH
            Size = result[0].Size
            TenMau = result[0].TenMau
            Min = result[0].Min == null ? FindMin(result[0].SttThung) : result[0].Min
            Max = result[0].Max == null ? FindMax(result[0].SttThung) : result[0].Max
            GetDSThung(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
        }
        renderTbl_DS_KHDT(result)
    }
    else {
        renderTbl_DS_KHDT(ArrDS_KHDT)
    }
}
//
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
                <td>${x.TuThungDenThung}</td>
                <td class="totalThung"><span>${x.SLTH}</span>/${x.TongThung}</td>
                <td>${convertDatetime(x.NgayLapKH)}</td>
                
            `
            html += '</tr>'
        })

        tobdy_tbl_DS_KHDT.html(html);
    }
    else {
        tobdy_tbl_DS_KHDT.html(html);
    }
}

function removeSpace(key) {
    // Sử dụng biểu thức chính quy để loại bỏ tất cả các khoảng trắng
    const keyReplace = key.replace(/\s/g, '');
    return keyReplace;
}

function Option_slc(data) {
    let html = "";

    data.map((x, index) => {
        html += `
        <tr class="${index == 0 ? "active" : ""}" data-isdecat=${x.isDecat} data-mapkl=${x.MaPKL} data-MaHang = ${x.MaHang} 
        data-PO=${x.PO} data-displayname="${x.displayname}" data-madvsx=${x.MaDVSX} data-dotsx=${x.DotSX} data-poid="${x.POID}">
            <td>${x.displayname}</td>
        </tr>
        `
    })
    $("#slc_lenhsx").html(html);
}

function renderDsThung(data) {
    let html = "";
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
            <div class="thung-item ${x.IsNhapKho == true ? "nhapkho" : ""}" data-sttThung=${x.SttThung} data-thunghienthi=${stt} id="thung${x.SttThung}">
                <p>${stt}</p>
                ${x.IsDongThung == false ? img_chuaDongThung : img_daDongThung}
                ${typeAction == 1 ? `${x.IsNhapKho == false ? `<input type="checkbox" />` : `<input type="checkbox" checked disabled />`}` : `${(x.IsDongThung == false) ? `<input type="checkbox" />` : `<input type="checkbox" checked disabled />`}`}
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
                    content: table
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
                    allowHTML: true
                });
            }
        });


    }
    else {
        if (typeAction == 1) {
            $("#dsThung").html("<h3>Thùng chưa được đóng</h3>");
        }
        else {
            $("#dsThung").html("<h3>Thùng đã được đóng hết</h3>");
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

function CountDongThung(data) {
    const count = data.reduce((acc, curr) => {
        if (curr.IsDongThung == true) {
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
    GetDSThung(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
})

//slc_lenhsx.on("click", "tr", function () {

//})


$("body #dsThung").on("click", ".thung-item input", function () {
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
        arrSave.push(newObj)
    } else {
        arrSave = arrSave.filter(function (item) {
            return item.SttThung !== SttThung;
        });
    }
})

//handleDongThung
handleDongThung.on("click", function () {
    if (typeAction == 1) {
        // Cập nhật tất cả các phần tử trong mảng arrSave cho trường NgayLapKH
        const updatedArrSave = arrSave.map(item => ({
            ...item, // Giữ nguyên các trường dữ liệu khác nếu có
            NgayDongThung: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
            IsDongThung: 0
        }));
        if (updatedArrSave.length > 0) {
            UpdateStatus(updatedArrSave)
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
            NgayDongThung: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
            IsDongThung: 1
        }));
        if (updatedArrSave.length > 0) {
            UpdateStatus(updatedArrSave)
        }
        else {
            Toast.fire({
                icon: 'error',
                title: 'Chưa chọn thùng để đóng'
            })
        }
    }
})

$("#btn-accept").on("click", function () {
    let arrFilter = []
    arrFilter = ArrDsDongThung.filter(function (item) {
        return item.IsDongThung == 0;
    });
})

handleFast.on("click", function () {
    let arrFilter = []
    if (typeAction == 1) {
        arrFilter = ArrDsDongThung.filter(function (item) {
            return item.IsDongThung == 1 && item.IsNhapKho == 0;
        });
        if (arrFilter.length == 0) {
            Toast.fire({
                icon: 'warning',
                title: 'Không có thùng để hủy'
            })
        }
        else {
            swalWithBootstrapButtons.fire({
                title: "Hủy đóng toàn bộ thùng!",
                text: "Cảnh báo!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Đồng ý",
                cancelButtonText: "Hủy",
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    let newArray = arrFilter.map(({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayDongThung }) => ({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayDongThung }));
                    const updatedArrSave = newArray.map(item => ({
                        ...item, // Giữ nguyên các trường dữ liệu khác nếu có
                        SttThung: item.SttThung,
                        MaDH: MaDH,
                        MaPKL: MaPKL,
                        MaDVSX: MaDVSX,
                        POID: POID,
                        NgayDongThung: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
                        IsDongThung: 0
                    }));
                    UpdateStatus(updatedArrSave)
                }
            });
        }
    }
    else {
        arrFilter = ArrDsDongThung.filter(function (item) {
            return item.IsDongThung == 0;
        });
        if (arrFilter.length == 0) {
            Toast.fire({
                icon: 'warning',
                title: 'Thùng đã được đóng hết'
            })
        }
        else {
            swalWithBootstrapButtons.fire({
                title: "Đóng toàn bộ thùng!",
                text: "Cảnh báo!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Đồng ý",
                cancelButtonText: "Hủy",
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    let newArray = arrFilter.map(({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayDongThung }) => ({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayDongThung }));
                    const updatedArrSave = newArray.map(item => ({
                        ...item, // Giữ nguyên các trường dữ liệu khác nếu có
                        SttThung: item.SttThung,
                        MaDH: MaDH,
                        MaPKL: MaPKL,
                        MaDVSX: MaDVSX,
                        POID: POID,
                        NgayDongThung: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
                        IsDongThung: 1
                    }));
                    UpdateStatus(updatedArrSave)
                }
            });
        }
    }
})

$("#handleByQuantity").on("click", function () {
    let arrFilter = []
    arrFilter = ArrDsDongThung.filter(function (item) {
        return item.IsDongThung == 0;
    });
    if (arrFilter.length == 0) {
        Toast.fire({
            icon: 'warning',
            title: 'Thùng đã được đóng hết'
        })
    }
    else {
        Swal.fire({
            title: "Nhập số lượng thùng",
            input: "number",
            inputAttributes: {
                autocapitalize: "off"
            },
            showCancelButton: true,
            cancelButtonText: "Hủy",
            confirmButtonText: "Xác nhận",
            showLoaderOnConfirm: true,
            allowOutsideClick: false,
        }).then((result) => {
            if (result.isConfirmed) {
                let SoLuong = result.value;
                if (SoLuong > 0) {
                    let newArray = arrFilter.slice(0, SoLuong).map(({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayDongThung }) => ({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayDongThung }));
                    const updatedArrSave = newArray.map(item => ({
                        ...item, // Giữ nguyên các trường dữ liệu khác nếu có
                        SttThung: item.SttThung,
                        MaDH: MaDH,
                        MaPKL: MaPKL,
                        MaDVSX: MaDVSX,
                        POID: POID,
                        NgayDongThung: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
                        IsDongThung: 1
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
//
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
        if (typeAction == 1) {
            arrFilter = ArrDsDongThung.filter(function (item) {
                return item.IsDongThung == 1 && item.IsNhapKho == 0;
            });
            if (arrFilter.length == 0) {
                Toast.fire({
                    icon: 'warning',
                    title: 'Không có thùng để hủy'
                })
            }
            else {
                let arrFilter1 =[]; // = arrFilter.filter(item => item.SttThung_temp >= ViTriBD && item.SttThung_temp <= ViTriKT)

                var valueCheckDecat = arrFilter[0].SttThung_decat;
                if (valueCheckDecat != "0")
                    arrFilter1 = arrFilter.filter(item => item.SttThung_decat >= ViTriBD && item.SttThung_decat <= ViTriKT)
                else
                    arrFilter1 = arrFilter.filter(item => item.SttThung_temp >= ViTriBD && item.SttThung_temp <= ViTriKT)
                console.log(arrFilter1, "arrFilter1")
                if (arrFilter1.length > 0) {
                    let newArray = arrFilter1.map(({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayDongThung }) => ({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayDongThung }));
                    const updatedArrSave = newArray.map(item => ({
                        ...item, // Giữ nguyên các trường dữ liệu khác nếu có
                        SttThung: item.SttThung,
                        MaDH: MaDH,
                        MaPKL: MaPKL,
                        MaDVSX: MaDVSX,
                        POID: POID,
                        NgayDongThung: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
                        IsDongThung: 0
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
        else {
            var valueCheckDecat = ArrDsDongThung[0].SttThung_decat;            
            if (valueCheckDecat != "0") 
                arrFilter = ArrDsDongThung.filter(item => item.SttThung_decat >= ViTriBD && item.SttThung_decat <= ViTriKT)
            else
                arrFilter = ArrDsDongThung.filter(item => item.SttThung_temp >= ViTriBD && item.SttThung_temp <= ViTriKT)
            if (arrFilter.length > 0) {
                let newArray = arrFilter.map(({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayDongThung }) => ({ SttThung, MaDH, MaPKL, MaDVSX, POID, NgayDongThung }));

                const updatedArrSave = newArray.map(item => ({
                    ...item, // Giữ nguyên các trường dữ liệu khác nếu có
                    SttThung: item.SttThung,
                    MaDH: MaDH,
                    MaPKL: MaPKL,
                    MaDVSX: MaDVSX,
                    POID: POID,
                    NgayDongThung: formattedDateTime, // Cập nhật giá trị cho trường NgayLapKH
                    IsDongThung: 1
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

    $("#ViTriBD").val("");
    $("#ViTriKT").val("");
})
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
    return $.ajax({
        url: '/api/DongThung/PostDongThung',
        type: 'post',
        dataType: 'json',
        data: JSON.stringify(arr),
        contentType: 'application/json',
        success: function (data) {
            isSave = true;
            if (typeAction == 1) {
                Toast.fire({
                    icon: 'success',
                    title: 'Hủy đóng thùng thành công'
                })
            }
            else {
                Toast.fire({
                    icon: 'success',
                    title: 'Đóng thùng thành công'
                })
            }
            typeAction = 2
            GetDSThung(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)

            reset()
        }
    });
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
        MaHang = $(this).data("mahang")
        PO = $(this).data("po")
        POID = $(this).data("poid")
        DotSX = $(this).data('dotsx')
        isDecat = $(this).data("isdecat")
        GetDS_KHDT(MaPKL, MaHang, POID, StatusChuyen, MaDVSX, isDecat, DotSX)
        displayname = $(this).attr("data-displayname");

        let inputValue = displayname; // Giả sử data[0].displayname là chuỗi HTML
        let textValue = $("<div>").html(inputValue).text(); // Chuyển đổi từ HTML sang văn bản thô
        input.val(textValue);
        reset()

        isHide = false;
        tobdy_tbl_DS_KHDT.show()
        checkHide()


    });

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
        if (isLuuChuyen) {
            StatusChuyen = 1
        }
        else {
            StatusChuyen = 0
        }
        GetLenhSX(StatusChuyen)
        GetDS_KHDT(MaPKL, MaHang, POID, StatusChuyen, MaDVSX, isDecat, DotSX)
    })

    option.on("click", function () {
        let action = $(this).data("action")
        typeAction = action
        option.removeClass("active")
        $(this).addClass("active")
        let arrFilter = []
        if (action == "0") {
            arrFilter = ArrDsDongThung.filter(function (item) {
                return item.IsDongThung == 0;
            });
            select_selected.html("Chưa đóng")
            handleDongThung.html("Đóng thùng")
            handleFast.html("Đóng tất cả")
            handleByVitri.html("Đóng theo vị trí")
            handleByQuantity.css("display", "block")
        }
        else if (action == "1") {
            arrFilter = ArrDsDongThung.filter(function (item) {
                return item.IsDongThung == 1;
            });
            select_selected.html("Đã đóng")
            handleDongThung.html("Hủy đóng")
            handleFast.html("Hủy đóng tất cả")
            handleByVitri.html("Hủy đóng theo vị trí")
            handleByQuantity.css("display", "none")
        }
        else {
            arrFilter = ArrDsDongThung;
            select_selected.html("Tất cả")
            handleDongThung.html("Đóng thùng")
            handleFast.html("Đóng tất cả")
            handleByVitri.html("Đóng theo vị trí")
            handleByQuantity.css("display", "block")
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
        GetDSThung(MaPKL, POID, MaHang, Min, Max, MaDVSX, isDecat, DotSX)
        GetDS_KHDT(MaPKL, MaHang, POID, StatusChuyen, MaDVSX, isDecat, DotSX)
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
    //

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
