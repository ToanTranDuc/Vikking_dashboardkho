function loadDataThuHoi(mahang, malenh, makh) {
    const gridInstance = $("#grid-thu-hoi").dxDataGrid("instance");

    const startDate = $("#select-startdate").dxDateBox("instance").option("value");
    const endDate = $("#select-enddate").dxDateBox("instance").option("value");
    const formatDate = (date) => {
        if (!date) return null;
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    };

    LoadingSpinner.show();

    $.ajax({
        url: '/api/baocao/get',
        type: 'GET',
        data: {
            action: 'baocaothuhoi',
            param1: mahang,
            param2: malenh,
            param3: makh,
            param4: formatDate(startDate),
            param5: formatDate(endDate)
        },
        contentType: 'application/json',
        success: function (response) {

            if (response) {
                gridInstance.option("dataSource", response);
            } else {
                gridInstance.option("dataSource", []);
            }
        },
        error: function (error) {
            console.error("Lỗi khi lấy dữ liệu thu hồi:", error);
            DevExpress.ui.notify("Không thể lấy dữ liệu thu hồi NPL", "error", 3000);
        },
        complete: function () {
            LoadingSpinner.hide();
            $("#filter_mahang").show();
        }
    });
}

function loadDataCapThem(mahang, malenh, makh) {
    const gridInstance = $("#grid-cap-them").dxDataGrid("instance");

    const startDate = $("#select-startdate").dxDateBox("instance").option("value");
    const endDate = $("#select-enddate").dxDateBox("instance").option("value");
    const formatDate = (date) => {
        if (!date) return null;
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    };

    LoadingSpinner.show();

    $.ajax({
        url: '/api/baocao/get',
        type: 'GET',
        data: {
            action: 'baocaocapthem',
            param1: mahang,
            param2: malenh,
            param3: makh,
            param4: formatDate(startDate),
            param5: formatDate(endDate)
        },
        contentType: 'application/json',
        success: function (response) {

            if (response) {
                gridInstance.option("dataSource", response);
            } else {
                gridInstance.option("dataSource", []);
            }
        },
        error: function (error) {
            console.error("Lỗi khi lấy dữ liệu thu hồi:", error);
            DevExpress.ui.notify("Không thể lấy dữ liệu thu hồi NPL", "error", 3000);
        },
        complete: function () {
            LoadingSpinner.hide();
            $("#filter_mahang").show();
        }
    });
}

function loadDataCapThemNgoaiDH() {
    const gridInstance = $("#grid-cap-them-ngoai-don-hang").dxDataGrid("instance");

    const startDate = $("#select-startdate").dxDateBox("instance").option("value");
    const endDate = $("#select-enddate").dxDateBox("instance").option("value");
    const formatDate = (date) => {
        if (!date) return null;
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    };

    LoadingSpinner.show();

    $.ajax({
        url: '/api/baocao/get',
        type: 'GET',
        data: {
            action: 'baocaocapthemngoaidonhang',
            param4: formatDate(startDate),
            param5: formatDate(endDate)
        },
        contentType: 'application/json',
        success: function (response) {

            if (response) {
                gridInstance.option("dataSource", response);
            } else {
                gridInstance.option("dataSource", []);
            }
        },
        error: function (error) {
            console.error("Lỗi khi lấy dữ liệu thu hồi:", error);
            DevExpress.ui.notify("Không thể lấy dữ liệu thu hồi NPL", "error", 3000);
        },
        complete: function () {
            LoadingSpinner.hide();
            $("#filter_mahang").hide();
        }
    });
}