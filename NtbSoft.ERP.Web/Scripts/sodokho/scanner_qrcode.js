let scanner = null;
let isScanConsecutive = false;
let camList = $("#cam-list");
let _idCam = '';
let btnScanQR = $("#btn-scan-qr")
let camQrResult = $("#cam-qr-result")
let qrResult = $("#qr-result");
let mavitri3 = "";
let updateFlashAvailability = () => {
    scanner.hasFlash().then(hasFlash => {
    });
};
let issco = true
$('#btn-scan-qr').on('click', () => {
    $("#video").hide()
    issco = true
    btnScanQR.hidden = true;
    animbox.hidden = false;
    scanner = new QrScanner(video, result => setResult(camQrResult, result), {
        onDecodeError: error => {
            camQrResult.textContent = error;
        },
        //highlightScanRegion: true,
        //highlightCodeOutline: true,
    });
    scanner.start().then(() => {
        updateFlashAvailability();

        QrScanner.listCameras(true).then(cameras => cameras.forEach(camera => {
            const option = document.createElement('option');
            option.value = camera.id;
            option.text = camera.label;
            camList.add(option);
            //let index_cam0 = Array.from(camList.options).findIndex(option => option.text === "camera2 0, facing back");
            //camList.selectedIndex = index_cam0;
            //camList.dispatchEvent(new Event("change"));
        }));

        if (_idCam != '') {
            scanner.setCamera(_idCam).then(updateFlashAvailability);
        }
    });
});
//quét liên tục
//document.getElementById('scan_consecutive').addEventListener('change', (e) => {
//    const input = e.target;
//    isScanConsecutive = input.checked;
//});
function setResult(label, result) {
    if (issco) {
        outputData.innerHTML = result.data;
         mavitri3 = result.data
        qrResult.hidden = false;
        let rendersodo = function (result) {
            $(".text_list").empty();
            $(".tieude").empty()
            var parts = mavitri3.split('.');
            var parts1 = mavitri3.split('.');
            parts1[2] = "00";
            parts[2] = "00";
            parts[1] = "00";
            var newValue = parts.join('.');
            var newValue1 = parts1.join('.');

            for (var i = 0; i < result.length; i++) {
                filteredData = $(result).filter(function (index, element) {
                    return element.MaVT === newValue;
                }).toArray();
                filteredData1 = $(result).filter(function (index, element) {
                    return element.MaVT === newValue1;
                }).toArray();
                filteredData2 = $(result).filter(function (index, element) {
                    return element.MaVT === mavitri3;
                }).toArray();

            }
            console.log(filteredData, "filteredData")
            console.log(filteredData1, "filteredData")
            console.log(filteredData2, "filteredData")
            let html2 = `
              Chi tiết ${filteredData[0].TenVT}  ${filteredData1[0].TenVT} ${filteredData2[0].TenVT}
            `
            $(".tieude").html(html2)
            let conlai = filteredData2[0].CBM - filteredData2[0].CBM_IsUsed
            let html = `
                <p>Sức chứa ${filteredData2[0].CBM} </p>
                <p>Đã chứa ${filteredData2[0].CBM_IsUsed} </p>
                <p>Còn lại ${conlai} </p>
        `
            $(".text_list").html(html)
        }

        let sodo = function () {
            let url = '/api/ViTriKho/GetShowVTK';
            $.ajax({
                url: url,
                type: "GET",
                data: {},
                contentType: 'application/json;charset=utf-8',
                success: function (data) {
                    rendersodo(data);
                }
            });
        }

        sodo();


        if (result.data.length > 0) {
            $(".popup").show()
            $("#video").hide()
            let chiTietO = function (vitri) {
                let url = '/api/ViTriKho/GetPN_VTK?action=GetPN_VTK&parameter=' + result.data;
                $.ajax({
                    url: url,
                    type: "GET",
                    data: {},
                    contentType: 'application/json;charset=utf-8',
                    success: function (data) {
                        renderChiTiet(data)
                       
                    }
                });
            }
            function handleInputSearch(selector, action) {
                $(selector).on("input", function (e) {
                    let keyup = $(this).val();
                    if ($(this).hasClass('list_input') && $(this).val() !== '') {
                        $('.list_input').not(this).val('');
                    }
                    if (keyup.includes("/")) {
                        alert("Xin vui lòng nhập dấu - thay dấu /");
                        keyup = keyup.replace('/', '-');
                        $(this).val(keyup);
                    }

                    var encodedKeyup = encodeURIComponent(keyup);
                    let url = '/api/ViTriKho/GetSearch?action=' + action + '&parameter=' + mavitri3 + '&parameter2=' + encodedKeyup;
                    $.ajax({
                        url: url,
                        type: "GET",
                        data: {},
                        contentType: 'application/json;charset=utf-8',
                        success: function (data) {
                            renderChiTiet(data)
                        }
                    });
                });
            }
            handleInputSearch(".search_maphieu", "SearchMaPhieuNhap");
            handleInputSearch(".search_ctvt", "SearchMaCTVT");
            handleInputSearch(".search_ngay", "SearchNgayNhapKho");
            function handleDate(_date) {
                var date = new Date(_date);
                return ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + date.getFullYear();
            }
            let renderChiTiet = function (data) {
                $("#tbody1").empty();
                if (data.length > 0) {
                    console.log(data, "data")
                    let html = "";

                    for (var i = 0; i < data.length; i++) {
                        let ngaythang1 = handleDate(data[i].NgayNhapKho)
                        html += `
                   <tr>
                   <td> ${data[i].MaPhieuNhap} </td>
                   <td> ${data[i].MaCTVT}</td>
                   <td> ${data[i].SoLuong}</td>
                   <td> ${data[i].TrongLuong}</td>
                   <td> ${data[i].KhoiLuong}</td>
                   <td> ${data[i].CBM}</td>
                   <td> ${ngaythang1}</td>
                   <td>
                    <input class="" type="checkbox" ${data[i].Status_PX.toString().toLowerCase() == "true" ? "checked" : ""} disabled>
                   </td>
                   
                  </tr>
                 `
                    }
                    $("#tbody1").html(html)
                }
            }

            chiTietO()
        }
      
        console.log(result.data, "SoDoKho/QRcode")
    }
    issco = false
}
$(".cancel_list").on("click", function () {
    $(".popup").hide()
})



//$('#btnLatCamera_Button').on('click', function () {
//    const selectElement = document.getElementById('cam-list');
//    const optionIndexMT = 0;
//    const optionIndexMS = 1;
//    var gtri = $('#textBool').val();
//    if (gtri == 1) {
//        var OptionMS = selectElement.options[optionIndexMS];
//        scanner.setCamera(OptionMS.value).then(updateFlashAvailability);
//        _idCam = event.target.value;
//        $('#textBool').val("2");
//    }
//    if (gtri == 2) {
//        var OptionMT = selectElement.options[optionIndexMT];
//        scanner.setCamera(OptionMT.value).then(updateFlashAvailability);
//        _idCam = event.target.value;
//        $('#textBool').val("1");
//    }
//});

//$(document).ready(function () {
//    var videoElement = document.getElementById('video');
//    var switchButton = $('#switchCamera');
//    var isFrontCamera = true;

//    // Hàm để chuyển đổi giữa camera trước và sau
//    function switchCamera() {
//        isFrontCamera = !isFrontCamera;

//        // Dừng video hiện tại
//        videoElement.pause();
//        videoElement.srcObject.getTracks().forEach(track => track.stop());

//        // Lấy constraints mới dựa trên loại camera muốn chuyển đổi
//        var constraints = {
//            video: { facingMode: (isFrontCamera ? 'user' : 'environment') }
//        };

//        // Bật camera mới
//        navigator.mediaDevices.getUserMedia(constraints)
//            .then(function (stream) {
//                videoElement.srcObject = stream;
//                videoElement.play();
//            })
//            .catch(function (error) {
//                console.error('Error accessing media devices.', error);
//            });
//    }

//    // Xử lý sự kiện khi nhấn nút chuyển đổi
//    switchButton.click(function () {
//        switchCamera();
//    });

//    // Khởi tạo và kích hoạt camera mặc định
//    navigator.mediaDevices.getUserMedia({ video: true })
//        .then(function (stream) {
//            videoElement.srcObject = stream;
//            videoElement.play();
//        })
//        .catch(function (error) {
//            console.error('Error accessing media devices.', error);
//        });
//});