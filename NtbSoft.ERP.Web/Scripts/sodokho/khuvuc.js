$(function () {
    let user = localStorage.getItem("username1")
    let nhaKho = function () {
        let url = '/api/ViTriKho/GetNhaMay?para1=' + user;
        $.ajax({
            url: url,
            type: "GET",
            data: {},
            contentType: 'application/json;charset=utf-8',
            success: function (data) {
                renderNhamay(data)
            }
        });
    }
    let renderNhamay = function (data) {
        let html = '';
        for (var i = 0; i < data.length; i++) {
            html += `
                <div class="item_hang" data-makho="${data[i].MaKho}">
                    <i class="fa-sharp fa-solid fa-warehouse"></i>  
                    <p>${data[i].TenKho}</p>
                </div>
            `
        }
        $(".list_hang").html(html)
    }
    nhaKho()
    $("body").on("click", '.item_hang', function () {
        let maKho = $(this).data('makho')
        nhaMay(maKho)

    })
    let nhaMay = function (maKho) {
        let url = '/api/ViTriKho/GetKhoVT_MK?nhamay=' + maKho;
        $.ajax({
            url: url,
            type: "GET",
            data: {},
            contentType: 'application/json;charset=utf-8',
            success: function (data) {
                if (data[0].MaVT == null) {
                    Swal.fire({
                        title: `Đơn vị này chưa được khai báo các kệ tầng và ô `,
                        icon: 'error',
                        confirmButtonText: 'Đồng ý',
                        confirmButtonColor: '#3085d6',
                        customClass: {
                            confirmButton: 'custom-confirm-button'
                        }
                    }).then((result) => {
                        if (result.isConfirmed) {
                            return;
                        }
                    });
                } else
                    window.location.href = '/SoDoKho/sodo?number=' + maKho;
            }
        });
    };
    let scanner = null;
    let isScanConsecutive = false;
    let camList = $("#cam-list");
    let _idCam = '';
    let btnScanQR = $("#btn-scan-qr")
    let camQrResult = $("#cam-qr-result")
    let qrResult = $("#qr-result");
    btnScanQR.on("click", function () {

        btnScanQR.hidden = true;
        animbox.hidden = false;
        scanner = new QrScanner(video, result => {
            setResult(camQrResult, result);
            const { boxes } = result;
            if (boxes && boxes.length > 0) {
                const qrPosition = boxes[0];
                console.log("QR Code Position:", qrPosition);
            }
        }, {
            onDecodeError: error => {
                camQrResult.textContent = error;
            },
            highlightScanRegion: true,
            highlightCodeOutline: true,
        });

        scanner.start().then(() => {
            updateFlashAvailability();

            QrScanner.listCameras(true).then(cameras => cameras.forEach(camera => {
                const option = document.createElement('option');
                option.value = camera.id;
                option.text = camera.label;
                camList.add(option);

                let index_cam0 = 0; // - set camera đầu tiên làm  camera cơ sở// Array.from(camList.options).findIndex(option => option.text === "camera2 0, facing back");
                camList.selectedIndex = index_cam0;
                //camList.dispatchEvent(new Event("change"));
                $('#textBool').val(index_cam0);
            }));


            if (_idCam != '') {
                scanner.setCamera(_idCam).then(updateFlashAvailability);
            }

        });
    })
    function setResult(label, result) {
        outputData.innerHTML = result.data;
        qrResult.hidden = false;

        console.log(result.data)
    }

    let updateFlashAvailability = () => {
        scanner.hasFlash().then(hasFlash => {
        });
    };

})