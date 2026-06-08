
var urlDonHang, lstMaHang, lstMaLenh;
var chooseMaLenh, choosePO;
// main
$(document).ready(function () {
    if (sessionStorage.getItem("BranchId") == null) {

    }
    else {
        if (sessionStorage.getItem("IsCefShap") != null)
            $('#BarMenu').hide();
        $('header-baocao ul.menu li').on('click', function () {
            if ($(this).find('ul.sub').css('display') == 'none') {
                $(this).find('ul.sub').css({ 'display': 'block' });
            }
        })
        removeListMaHang();
        removeListMaLenh();
        GetDS_MaHang();
    }
});
// load danh sách mã hàng
function GetDS_MaHang() {
    removeListMaHang();
    let BranchId = sessionStorage.getItem("BranchId");
    let data = RunStored_GetData(BranchId, "BaoCaoDongThungWeb", creatSqlParaDT("GetDSMaHang", "", "", "", "", ""));

    data.map((item, index) => {
        createLiMaHang(item);
    });

    $('.checkMH').on('change', function () {
        let mah = '';
        $('.checkMH').each(function () {
            if ($(this).prop('checked') == true) {
                mah += $(this).attr('mahang');
                mah += '@';
            }
        });
        $('#chooseHangHoa').val(mah);
    });
}
function loadDanhSachML() {
    removeListMaLenh();
    if ($('#AllMH').prop('checked') === false) {
        $('.checkMH').each(function () {
            if ($(this).prop('checked') === true) {
                lstMaHang += $(this).attr('mahang');
                lstMaHang += '@';
            }
        });
    }

    let BranchId = sessionStorage.getItem("BranchId");
    let data = RunStored_GetData(BranchId, "BaoCaoDongThungWeb", creatSqlParaDT("GetDSLenh", lstMaHang, "", "", "", ""));

    let count_tsl = 0;

    data.map((item, index) => {
        createLiLenh(item);
        count_tsl++;
    });

    $('#AllML').on('click', function () {
        let isChecked = $(this).prop('checked');
        $('.checkML').each(function () {
            if (!$(this).closest('li').hasClass('classHide'))
                $(this).prop('checked', isChecked);
        });
        setMaLenh();
    });

    let min = parseInt(data[count_tsl - 1].MaLenh);
    let max = parseInt(data[0].MaLenh);

    $('#tuLenh').attr('min', min);
    $('#denLenh').attr('min', min);

    $('#tuLenh').attr('max', max);
    $('#denLenh').attr('max', max);

    $('#tuLenh').val(min);
    $('#denLenh').val(max);

    $('#tuLenh').on('focusout change', function () {
        let valueT = parseInt($(this).val());
        if (valueT < min || valueT > max) {
            $(this).val(min);
            valueT = min;
        }
        addClassHideLenh();
    })
    $('#denLenh').on('focusout change', function () {
        let valueT = parseInt($(this).val());
        if (valueT < min || valueT > max) {
            $(this).val(max);
            valueT = max;
        }
        addClassHideLenh();
    })

    $('.checkML').on('change', function () {
        let mah = '';
        $('.checkML').each(function () {
            if ($(this).prop('checked') == true) {
                mah += $(this).attr('maLenh');
                mah += '@';
            }
        });
        $('#chooseProductOrder').val(mah);
    });
}


function GetDS_Mau(malenh, poid, element) {
    addClassAction(element);
    removeTrMau();
    chooseMaLenh = malenh;
    choosePO = poid;
    let spoid = malenh + '|' + poid;

    let BranchId = sessionStorage.getItem("BranchId");
    let data = RunStored_GetData(BranchId, "BaoCaoDongThungWeb", creatSqlParaDT("GetDSMau", spoid, "", "", "", ""));

    let count_data = 0;
    data.map((item, index) => {
        createTrMau(item);
        count_data++;
    });
    if (count_data > 0) {
        let item = data[0];
        Get_Result(item.DauSizeID, item.ColorID, $('.table-DS-Mau-tbody-tr').eq(0));
    }
}

function Get_Result(dausizeID, colorID, element) {
    addClassAction(element);
    removeTrResult();

    let BranchId = sessionStorage.getItem("BranchId");
    let data = GetData_BCDongThungRS(BranchId, "BaoCaoDongThungWeb", creatSqlParaDT("GetDS_Result", chooseMaLenh, choosePO, chooseMaLenh + "|" + choosePO, dausizeID, colorID));

    data.map((item, index) => {
        createTrResult(item);
    });
    sumresult();
}

// Xuất excel
$('#btnXuatExcel').on('click', function () {
    let BranchId = sessionStorage.getItem("BranchId");
    xuatFileExcel(BranchId, "BaoCaoDongThungWeb", creatSqlParaDT("GetDSTongQuan", lstMaLenh, "", "", "", ""));
});
function xuatFileExcel(server, stored, paramStr) {
    let dataResult = [];
    let DataJS = {
        server: server,
        stored: stored,
        paramStr: paramStr
    };
    if (sessionStorage.getItem("IsCefShap") == null) {
        $.ajax({
            url: '/api/sqlQuerryBCTD/ExportExcel',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(DataJS),
            //async: false, // Thiết lập xử lý đồng bộ
            xhrFields: {
                responseType: 'blob' // Nhận dữ liệu dưới dạng blob
            },
            success: function (data, status, xhr) {
                var blob = new Blob([data], { type: xhr.getResponseHeader('Content-Type') });
                var url = window.URL.createObjectURL(blob);
                var a = document.createElement("a");
                var fileName = `Báo cáo nhận TP-ĐT-NK ${getDateTimeNow()}`
                a.href = url;
                a.download = fileName;
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a); // Xóa phần tử a sau khi tải xong
            },
            error: function (xhr, status, error) {
                console.error('Error fetching data from the server.', error);
            }
        });
    } else {
        console.log(`ExportExcel@${JSON.stringify(DataJS)}`);
    }
}
//======================================================
// Khỏi tạo các element khi có data
//======================================================
// tạo li mã hàng
function createLiMaHang(item) {
    $('#liMaHang').append(`
        <li class="li-up-down">
            <div class="checkbox-wrapper-29">
                <label class="checkbox">
                <input type="checkbox" class="checkbox__input checkMH" mahang="${item.MaHang}" />
                <span class="checkbox__label"> </span>
                <span class="txtMaHang">${item.MaHang}</span>
                </label>
            </div>
        </li>
    `);
}
// tạo li mã lệnh
function createLiLenh(item) {
    $('#liLenhSX').append(`
        <li>
            <div class="checkbox-wrapper-29">
                <label class="checkbox">
                <input type="checkbox" class="checkbox__input checkML" maLenh="${item.MaLenh}" />
                <span class="checkbox__label"> </span>
                    Lệnh SX: ${item.MaLenh}
                </label>
            </div>
        </li>
    `);
}
// tạo tbody tổng quan
function createTrDSTongQuan(item) {
    $('#div-TongQuan-tbody').append(`
        <tr class="div-TongQuan-tbody-tr" onclick="GetDS_Mau('${item.MaLenh}', '${item.POID}', this);">
            <td class="divTQ-tbody-tr-malenh">${item.MaLenh}</td>
            <td poid="${item.POID}">${item.PO}</td>
            <td>${item.MaHang}</td>
            <td cusID="${item.MaKhachHang}">${item.KhachHang}</td>
            <td countryID="${item.MaQuocGia}">${item.QuocGia}</td>
            <td>${item.NgayGH}</td>
            <td class="sumTc">${item.SoLuong}</td>
            <td class="slnhan">${item.NhanTP}</td>
            <td class="slcl">${item.SoLuong - item.NhanTP}</td>
            <td class="slnhan">${item.NhanDT}</td>
            <td class="slcl">${item.SoLuong - item.NhanDT}</td>
            <td class="slnhan">${item.NhanNK}</td>
            <td class="slcl">${item.SoLuong - item.NhanNK}</td>
        </tr>
    `);
}
// tạo tbody màu
function createTrMau(item) {
    $('#table-DS-Mau-tbody').append(`
        <tr class="table-DS-Mau-tbody-tr" onclick="Get_Result('${item.DauSizeID}', '${item.ColorID}', this);" >
            <td >${item.DauSize}</td>
            <td>${item.MaMau}</td>
            <td>${item.Mau}</td>
        </tr>
    `);
}
// tạo tbody kết quả chi tiết
function createTrResult(item) {
    $('#table-DS-KetQua-tbody').append(`
         <tr>
            <td>${item.MaMau}</td>
            <td>${item.TenMau}</td>
            <td>${item.Size}</td>
            <td class="sumTc">${item.SoLuong}</td>
            <td class="sumTPN slnhan">${item.NhanTP}</td>
            <td class="sumTPCL slcl">${item.SoLuong - item.NhanTP}</td>
            <td class="sumDTN slnhan">${item.NhanDT}</td>
            <td class="sumDTCL slcl">${item.SoLuong - item.NhanDT}</td>
            <td class="sumNKN slnhan">${item.NhanNK}</td>
            <td class="sumNKCL slcl">${item.SoLuong - item.NhanNK}</td>
        </tr>
    `);
}

//====================================================== 
//Các function khác:
//======================================================
// tạo chuỗi lấy dữ liệu từ server
function creatSqlParaDT(action, para0, para1, para2, para3, para4) {
    return `@Action = '${action}', @para0 = '${para0}', @para1 = '${para1}',@para2 = '${para2}',@para3 = '${para3}',@para4 = '${para4}'`;
}
// lấy ngày tháng năm giờ phút giây hiện tại làm tên file
function getDateTimeNow() {
    var now = new Date();
    var day = ("0" + now.getDate()).slice(-2); // Lấy ngày và định dạng thành 2 chữ số
    var month = ("0" + (now.getMonth() + 1)).slice(-2); // Lấy tháng và định dạng thành 2 chữ số
    var year = now.getFullYear(); // Lấy năm
    var hours = ("0" + now.getHours()).slice(-2); // Lấy giờ và định dạng thành 2 chữ số
    var minutes = ("0" + now.getMinutes()).slice(-2); // Lấy phút và định dạng thành 2 chữ số
    var seconds = ("0" + now.getSeconds()).slice(-2); // Lấy giây và định dạng thành 2 chữ số

    var formattedDateTime = day + '/' + month + '/' + year + ' ' + hours + ':' + minutes + ':' + seconds;

    return formattedDateTime;
}
// set lại mã lệnh cần lấy
function setMaLenh() {
    lstMaLenh = '@@';
    $('.checkML').each(function () {
        if ($(this).prop('checked') === true) {
            lstMaLenh += $(this).attr('maLenh');
            lstMaLenh += '@';
        }
    });
    $('#chooseProductOrder').val(lstMaLenh);
}
// Tô màu cho thẻ tr của tổng quan
function tomauTr() {
    let isML = 0;
    let ML = "ML"
    $('.divTQ-tbody-tr-malenh').each(function () {
        let malenhtr = $(this).text();
        if (malenhtr == ML) {

        } else {
            isML++;
            ML = malenhtr;
        }
        if (isML % 2 == 0) {
            $(this).closest('tr').addClass('classChan');
        }
        else {
            $(this).closest('tr').addClass('classLe');
        }
    });
}
// loại bỏ tbody khi load lại data
function removeTrTongQuan() {
    $('#div-TongQuan-tbody').empty();
}

function removeTrMau() {
    $('#table-DS-Mau-tbody').empty();
}
function removeTrResult() {
    $('#table-DS-KetQua-tbody').empty();
    $('#sumT').text(0);
    $('#sumTPN').text(0);
    $('#sumTPCL').text(0);
    $('#sumDTN').text(0);
    $('#sumDTCL').text(0);
    $('#sumNKN').text(0);
    $('#sumNKCL').text(0);
}
// loại bỏ danh sách mã hàng - sử dụng khi muons load lại dữ liệu
function removeListMaHang() {
    $('#liMaHang').empty();
    $('#liMaHang').append(`
        <li class="li-pc">
                        <div id="div-search-mahang">
                            <input id="search_input" type="search" placeholder="Tìm kiếm mã hàng" />
                            <button id="btnXacNhanMH" onclick="">Xác nhận</button>
                            <div class="checkbox-wrapper-29" style="background-color: white; font-size:12px;">
                                <label class="checkbox">
                                    <input type="checkbox" class="checkbox__input" id="AllMH" />
                                    <span class="checkbox__label" style="translate: 5px -5px;"> </span>
                                    Chọn hết
                            </label>
                       </div>
                 </div>
            </li>
     `);
    $('#btnXacNhanMH').on('click', function () {
        $('#liMaHang').fadeOut(500);
        lstMaHang = '@@';
        loadDanhSachML();
    });
    $('#AllMH').on('click', function () {
        let isChecked = $(this).prop('checked');
        $('.checkMH').each(function () {
            if (!$(this).closest('li').hasClass('classHide'))
                $(this).prop('checked', isChecked);
        });
    });

    $('#search_input').on('input', function () {
        let inputStr = $(this).val().toLowerCase();
        $('.txtMaHang').each(function () {
            let maHangStr = $(this).text().toLowerCase();
            let liMaHangStr = $(this).closest('li');
            if (maHangStr.includes(inputStr) === false) {
                $(liMaHangStr).addClass('classHide');
            } else {
                $(liMaHangStr).removeClass('classHide');
            }
        });
    });
}
// loại bỏ danh sách mã lệnh
function removeListMaLenh() {
    $('#liLenhSX').empty();
    $('#liLenhSX').append(`
        <li class="li-pc">
            <div id="tuLenhDenLenh">
                <span>Từ:&nbsp;</span><input type="number" id="tuLenh" />
                <span>&nbsp;Đến:&nbsp;</span><input type="number" id="denLenh" />
                &nbsp;
                <div class="checkbox-wrapper-29" style="background-color: white; font-size: 12px; translate: 0 5px; ">
                    <label class="checkbox">
                        <input type="checkbox" class="checkbox__input" id="AllML" />
                        <span class="checkbox__label" style="translate: 5px -3px;"> </span>
                        Chọn hết
                    </label>
                    <button id="btnXacNhanMaLenh">Xác nhận</button>
                </div>
            </div>
        </li>
    `);
    $('#btnXacNhanMaLenh').on('click', function () {
        $(this).closest('ul.sub').fadeOut(500);
        removeTrTongQuan();
        let BranchId = sessionStorage.getItem("BranchId");
        let data = GetData_BCDongThung(BranchId, "BaoCaoDongThungWeb", creatSqlParaDT("GetDSTongQuan", lstMaLenh, "", "", "", ""));

        let count_data = 0;
        if (data == null) {
            Swal.fire({
                title: 'Thông báo',
                text: 'Không có dữ liệu ở thời gian được chọn!',
                icon: 'info',
                confirmButtonText: 'OK'
            });
            return;
        }
        data.map((item, index) => {
            createTrDSTongQuan(item);
            count_data++;
        });
        if (count_data > 0) {
            let item = data[0];
            GetDS_Mau(item.MaLenh, item.POID, $('.div-TongQuan-tbody-tr').eq(0));
        }
        tomauTr();
    });
}
// Tính tổng
function sumresult() {
    let sumTongSL = 0;
    let sumTPNSL = 0;
    let sumTPCLSL = 0;
    let sumDTNSL = 0;
    let sumDTCLSL = 0;
    let sumNKNSL = 0;
    let sumNKCLSL = 0;

    $('#table-DS-KetQua-tbody').find('.sumTc').each(function () {
        sumTongSL += parseInt($(this).text());
    });
    $('.sumTPN').each(function () {
        sumTPNSL += parseInt($(this).text());
    });
    $('.sumTPCL').each(function () {
        sumTPCLSL += parseInt($(this).text());
    });
    $('.sumDTN').each(function () {
        sumDTNSL += parseInt($(this).text());
    });
    $('.sumDTCL').each(function () {
        sumDTCLSL += parseInt($(this).text());
    });
    $('.sumNKN').each(function () {
        sumNKNSL += parseInt($(this).text());
    });
    $('.sumNKCL').each(function () {
        sumNKCLSL += parseInt($(this).text());
    });

    $('#sumT').text(sumTongSL);
    $('#sumTPN').text(sumTPNSL);
    $('#sumTPCL').text(sumTPCLSL);
    $('#sumDTN').text(sumDTNSL);
    $('#sumDTCL').text(sumDTCLSL);
    $('#sumNKN').text(sumNKNSL);
    $('#sumNKCL').text(sumNKCLSL);
}
// ẩn / hiện các item mã lệnh
function addClassHideLenh() {
    $('.checkML').each(function () {
        let valT = parseInt($(this).attr('malenh'));
        let min = parseInt($('#tuLenh').val());
        let max = parseInt($('#denLenh').val());
        if (valT < min || valT > max) {
            $(this).closest('li').addClass('classHide');
        } else {
            $(this).closest('li').removeClass('classHide');
        }
    })
}

function addClassAction(trElement) {
    $(trElement).closest('tbody').find('tr').each(function () {
        $(this).removeClass('classAction');
    });
    $(trElement).closest('tr').addClass('classAction');
}
// =================== Load data =============================
// Lấy thông tin đóng thùng tổng quan
function GetData_BCDongThung(server, stored, paramStr) {
    let dataResult = [];
    let DataJS = {
        server: server,
        stored: stored,
        paramStr: paramStr
    };

    $.ajax({
        url: '/api/sqlQuerryBCTD/getTongQuan',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(DataJS),
        async: false, // Thiết lập xử lý đồng bộ
        success: function (data) {
            dataResult = data; // Gán trực tiếp kết quả vào dataResult
        },
        error: function () {
            // Xử lý lỗi ở đây, ví dụ: hiển thị thông báo lỗi
            console.error('Lỗi khi lấy dữ liệu');
        }
    });
    return dataResult;
}
// lấy thông tin đóng thùng chi tiết 
function GetData_BCDongThungRS(server, stored, paramStr) {
    let dataResult = [];
    let DataJS = {
        server: server,
        stored: stored,
        paramStr: paramStr
    };

    $.ajax({
        url: '/api/sqlQuerryBCTD/getResult',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(DataJS),
        async: false, // Thiết lập xử lý đồng bộ
        success: function (data) {
            dataResult = data; // Gán trực tiếp kết quả vào dataResult
        },
        error: function () {
            // Xử lý lỗi ở đây, ví dụ: hiển thị thông báo lỗi
            console.error('Lỗi khi lấy dữ liệu');
        }
    });

    return dataResult;
}