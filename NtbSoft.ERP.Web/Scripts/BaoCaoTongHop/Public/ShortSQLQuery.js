
// Sử lí đồng bộ
/**
 * Lấy data từ API bằng cách viết trực tiếp câu truy vấn
 * @param {any} queryJS
 */
function ShortSQLQuery_GetData(queryJS) {
    let dataResult = [];
    let DataJS = {
        query: queryJS,
    };

    $.ajax({
        url: '/api/sqlQ/getObj',
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

function RunStored_GetData(server, stored, paramStr) {
    let dataResult = [];
    let DataJS = {
        server: server,
        stored: stored,
        paramStr: paramStr
    };

    $.ajax({
        url: '/api/sqlQ/RunStored',
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
function RunStored_GetData_DH(server, stored, paramStr) {
    let dataResult = [];
    let DataJS = {
        server: server,
        stored: stored,
        paramStr: paramStr
    };

    $.ajax({
        url: '/api/sqlQ/RunStoredDH',
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
function getRoleDVSX(dvsx) {
    let param = `@action = 'get_role_DVSX',@user = '${localStorage.getItem("username1")}', @branch = '${dvsx}'`;
    let quyenVaoDVSX = RunStored_GetData("main", "user_role_BC_Line", param);
    if (quyenVaoDVSX == null)
        return false;
    if (quyenVaoDVSX.length > 0)
        return true;
    return false;
}
function getRoleBC() {
    let param = `@action = 'get_role_BC',@user = '${localStorage.getItem("username1")}', @branch = ''`;
    let quyenVaoBC = RunStored_GetData("main", "user_role_BC_Line", param);
    if (quyenVaoBC == null)
        return false;
    return quyenVaoBC;
}
function getRoleLine(dvsx) {
    let param = `@action = 'get_role_Line',@user = '${localStorage.getItem("username1")}', @branch = '${dvsx}'`;
    let get_role_Line = RunStored_GetData("main", "user_role_BC_Line", param);
    if (get_role_Line == null)
        return false;
    return get_role_Line;
}

/*
 
M.09.04.00	bao-cao-tong-quan-san-xuat
M.09.05.00	bao-cao-tien-do
M.09.06.00	bao-cao-nang-suat-doanh-thu
M.09.07.00	bao-cao-chat-luong-ma-hang-tren-chuyen
M.09.08.00	bao-cao-chat-luong-ma-hang-sau-ui
M.09.09.00	bao-cao-nhap-kho-TP
M.09.10.00	bao-cao-dong-thung
M.09.11.00	bao-cao-ke-hoach-cat-ngay
M.09.12.00	bao-cao-ti-le-ma-hang-loi-chuyen
M.09.13.00	bao-cao-thoi-gian-hao-phi
M.09.14.00	bao-cao-nhan-luc

 */