function GetKeyValueWebConfig(keyValue) {
    let dataResult;
    let DataJS = {
        query: keyValue,
    };

    $.ajax({
        url: '/api/webconfig/GetKey',
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
