var maPhieuXH = '';
var Cont = '';
var BarCodeCheck = '';
var dtData = [];
var lstScaned = [];
let html5QrCode = new Html5Qrcode("reader");
var dtBarCode = [];
var html5QrcodeScanner = new Html5QrcodeScanner(
    "reader", { fps: 10, qrbox: 250 });
html5QrcodeScanner.render(onScanSuccess, onScanError);
const qrConfig = { fps: 10, qrbox: { width: 300, height: 300 } };
const brConfig = { fps: 10, qrbox: { width: 300, height: 150 } };
var flagPrevent = false;
var userName = '';
var flagLock = false;
$(document).ready(function () {
    userName = localStorage.getItem('username');
    html5QrcodeScanner.clear();
    getPhieuXH();    
    setInterval(GetData, 1000);
    // getPhieuXH();
})
function getDate() {

}
function getPhieuXH() {
    var fromdate = '2024-01-28';
    var todate = '2025-01-01'
    $.ajax({
        url: `/api/XuatHang/Get?action=GetPhieuXH&Para1=${fromdate}&Para2=${todate}&Para3=A`,
        type: 'GET',
        success: function (data) {
            $.each(data, function (key, items) {
                $('#MaPKLXH').append($('<option></option>').val(items.value).html(items.MaPKLXH_Display));
            })
            if (data.length > 0) $('#MaPKLXH').val(data[0].value);
            PhieuXHChange();            
        }
    })
}
function PhieuXHChange() {
   // flagLock = true;
    maPhieuXH = $('#MaPKLXH').val().split('|')[0];
    Cont = $('#MaPKLXH').val().split('|')[1];
    GetBarcode();
    //GetData(1);
    
}
//function HandleTimer() {
//    if (flagLock) {       
//        return;
//    }
//    GetData(0);
//}
function GetData() {
    $.ajax({
        url: `/api/XuatHang/Get?action=GetCTPhieuXH&Para1=${maPhieuXH}&Para2=${Cont}&Para3=A`,
        type: 'GET',
        success: function (data) {           
            dtData = data.filter(x => x.Cont == Cont);
            if (data == null || data.length == 0) return;
            RenderTable(dtData);
            var dtSumBarcode = dtData.filter(x => x.Barcode == BarCodeCheck);
            var sumSTDaQuet = dtSumBarcode.reduce((n, { STDaQuet }) => n + STDaQuet, 0);
            var sumSTKH = dtSumBarcode.reduce((n, { SLThung }) => n + SLThung, 0);
            document.getElementById('STTong_S').innerHTML = sumSTDaQuet + "/" +sumSTKH
        }
    })
}
function GetBarcode() {
    $.ajax({
        url: `/api/XuatHang/Get?action=GetBarcode&Para1=${maPhieuXH}&Para2=${Cont}&Para3=A`,
        type: 'GET',
        success: function (data) {
            dtBarCode = data;
            BingdingPO(data);
        }
    })
}
function BingdingPO() {
    var lstPO = [...new Set(dtBarCode.map(item => item.POID))];
    $.each(lstPO, function (key, POID) {
        var lstdataPO = $.grep(dtBarCode, function (e) { return e.POID == POID })
        $('#PO').append($('<option></option>').val(POID).html(lstdataPO[0].PO));
    })
    POChange();
}
function POChange() {
    BingingDauSizeID();
}
function BingingDauSizeID() {
    $('#DauSize').html('')
    var lstdataPO = $.grep(dtBarCode, function (e) { return e.POID == $('#PO').val() })
    var lstDauSize = [...new Set(lstdataPO.map(item => item.DauSizeID))];
    $.each(lstDauSize, function (key, DauSizeID) {
        var lstdataDauSize = $.grep(lstdataPO, function (e) { return e.DauSizeID == DauSizeID })
        $('#DauSize').append($('<option></option>').val(DauSizeID).html(lstdataDauSize[0].DauSize));
    })
    DauSizeChange();
}
function DauSizeChange() {
    BingdingMau();
}
function BingdingMau() {
    $('#Mau').html('')
    var lstdata = $.grep(dtBarCode, function (e) { return e.POID == $('#PO').val() && e.DauSizeID == $('#DauSize').val() })
    var lstMau = [...new Set(lstdata.map(item => item.ColorID))];
    $.each(lstMau, function (key, ColorID) {
        var lstdataMau = $.grep(lstdata, function (e) { return e.ColorID == ColorID })
        $('#Mau').append($('<option></option>').val(ColorID).html(lstdataMau[0].TenMau));
    })
    MauChange();
}
function MauChange() {
    BingdingSize();
}
function BingdingSize() {
    $('#Size').html('')
    var lstdata = $.grep(dtBarCode, function (e) { return e.POID == $('#PO').val() && e.DauSizeID == $('#DauSize').val() && e.ColorID == $('#Mau').val() })
    var lstSize = [...new Set(lstdata.map(item => item.SizeID))];
    $.each(lstSize, function (key, SizeID) {
        var lstdataSize = $.grep(lstdata, function (e) { return e.SizeID == SizeID })
        $('#Size').append($('<option></option>').val(SizeID).html(lstdataSize[0].Size));
    })
    SizeChange();
   
}
function SizeChange() {
    var lstTemp = $.grep(dtBarCode, function (e) { return e.POID == $('#PO').val() && e.DauSizeID == $('#DauSize').val() && e.ColorID == $('#Mau').val() && e.SizeID == $('#Size').val() })
    BarCodeCheck = lstTemp[0].Barcode;
    BindingCardScanN();
    ConfirmSize();
}
function ConfirmSize() {
    let ojSave = {       
        MaPKL_XH: maPhieuXH,
        POID: $('#PO').val(),
        PO: $("#PO option:selected").text(),
        DauSizeID: $('#DauSize').val(),
        DauSize: $("#DauSize option:selected").text(),
        ColorID: $('#Mau').val(),
        TenMau: $("#Mau option:selected").text(),
        SizeID: $('#Size').val(),
        Size: $("#Size option:selected").text(),
        Cont: Cont,
        Barcode: BarCodeCheck,
        NVien: userName,

    }
    $.ajax({
        url: '/api/XuatHang/UpdateBarcodeLink?Action=UpdateBarcodeLink',
        type: 'Post',
        dataType: 'json',
        data: JSON.stringify(ojSave),
        contentType: 'application/json',
        success: function (data) {
           
        }
    });
}
function Save(ojSave) {

    $.ajax({
        url: '/api/XuatHang/UpdateScan?Action=UpdateScan',
        type: 'Post',
        dataType: 'json',
        data: JSON.stringify(ojSave),
        contentType: 'application/json',
        success: function (data) {
            if (data == "false") {
                dtData.map((x, index) => {
                    if (x.Barcode == ojSave.Barcode) {
                        x.STDaQuet = x.STDaQuet == 0 ? 0 : x.STDaQuet - 1;
                        BindingCardScan(x);
                    }
                });
            }
            RenderTable(dtData);
        }
    });
}
function RenderTable(data) {
    var htmlHeader1 = '<th>Từ thùng</th><th>Đến thùng</th>';
    var htmlBody = '';
    var lstFieldName = Object.keys(data[0]);
    var lstSize = [];
    var count = 0;
    lstFieldName.map((x, index) => {
        if (x.includes('@')) {
            var value = x.split('@');
            htmlHeader1 += `<th>${value[0]}</th>`;
            lstSize.push(x);
            count++;
        }
    });
    var cell = document.getElementById("colSize");
    cell.colSpan = count;
    $('#lstSize').html(htmlHeader1)
    data.map((x, index) => {
        htmlBody += `<tr><td>${x.MaPKLDisplay}</td>
                                 <td>${x.TuThung}</td>
                                 <td>${x.DenThung}</td>
                                 <td>${x.MaHang}</td>
                                 <td>${x.PO}</td>
                                 <td>${x.DauSize}</td>
                                 <td>${x.TenMau}</td>
                                 <td>${x.MaCont}</td>`
        lstSize.map((x1, index1) => {
            htmlBody += `<td>${x[x1] == "0" ? "" : x[x1]}</td>`
        });
        htmlBody += `<td>${x.SoLuong}</td>
                   
                             <td>${x.SLThung}</td>
                             <td>${x.STDaQuet}</td>
          <td>${x.TotalPiece}</td>
                             <td>${x.Barcode}</td></tr>`
    });
    $('#tblBodyCT').html(htmlBody);
}

// Config Bar/QR code

$("#scanQR").on("click", function () {
    if ($("#scanQR").hasClass("active")) {
        html5QrCode
            .stop()
            .then((res) => {
                html5QrCode.clear();
            })
            .catch((err) => {
                console.log(err.message);
            });
        $(this).removeClass("active")
        $(this).text("Quét mã")
    }

    else {
        $(this).addClass("active")
        html5QrCode.start(
            { facingMode: "environment" },
            qrConfig,
            qrCodeSuccessCallback
        );
        $(this).text("Tắt quét mã")
    }
})
var myTimeout;
const qrCodeSuccessCallback = (qrCodeMessage) => {

    if (flagPrevent) return;
    var audio = new Audio('/img/scannerAudio.mp3');
    audio.play();
    flagPrevent = true;
    myTimeout = setTimeout(HandleTimeOut, 2000);
    var drCheckBarCode = dtData.filter(x => x.Barcode.trim() == qrCodeMessage.trim());
    if (drCheckBarCode.length <= 0) {
        Swal.fire({
            title: `Vui lòng kiểm tra lại barcode đã được khai báo chưa!`,
            icon: 'warning',
            showConfirmButton: false,
            timer: 2500, // Thời gian hiển thị thông báo (1500 ms = 1.5 giây)
            timerProgressBar: true, // Hiển thị thanh tiến trình của timer
            allowOutsideClick: false,
            backdrop: true,
            didOpen: () => {
                Swal.showLoading(); // Hiển thị thanh tiến trình khi thông báo đang được hiển thị
            }
        });
        return;
    }
    var drCheckThung = dtData.filter(x => x.Barcode.trim() == qrCodeMessage.trim() && x.STDaQuet + 1 <= x.SLThung);
    if (drCheckThung.length <= 0) {
        Swal.fire({
            title: `SL Thùng đã vượt số lượng!`,
            icon: 'warning',
            showConfirmButton: false,
            timer: 2500, // Thời gian hiển thị thông báo (1500 ms = 1.5 giây)
            timerProgressBar: true, // Hiển thị thanh tiến trình của timer
            allowOutsideClick: false,
            backdrop: true,
            didOpen: () => {
                Swal.showLoading(); // Hiển thị thanh tiến trình khi thông báo đang được hiển thị
            }
        });
        BindingCardScan(drCheckBarCode[0]);
        return;
    }

    for (let i = 0; i < dtData.length; i++) {
        var x = dtData[i]
        if (x.Barcode.trim() == qrCodeMessage.trim()) {
            if ((x.STDaQuet + 1) <= x.SLThung) {
                x.STDaQuet = x.STDaQuet + 1;
                BindingCardScan(x);
                Save(x);
                break;
            }
        }
    }
    RenderTable(dtData);

}
function BindingCardScanN() {
   // document.getElementById('MaHang_S').innerHTML = "&nbsp" + x.MaHang
    document.getElementById('PO_S').innerHTML = "&nbsp" + $("#PO option:selected").text()
    //document.getElementById('STTong_S').innerHTML = "&nbsp" + x.STDaQuet + '/' + x.SLThung
    document.getElementById('DauSize_S').innerHTML = "&nbsp" + $("#DauSize option:selected").text()
    document.getElementById('Mau_S').innerHTML = "&nbsp" + $("#Mau option:selected").text()
    //var drSize = dtBarCode.filter(y => y.Barcode == BarCodeCheck)[0];
    document.getElementById('Size_S').innerHTML = "&nbsp" + $("#Size option:selected").text()
    // document.getElementById('SLScan_S').innerHTML = "&nbsp" + x.STDaQuet +'/'+ x.SLThung
}
function BindingCardScan(x) {
    document.getElementById('MaHang_S').innerHTML = "&nbsp" + x.MaHang
    document.getElementById('PO_S').innerHTML = "&nbsp" + x.PO
    document.getElementById('STTong_S').innerHTML = "&nbsp" + x.STDaQuet + '/' + x.SLThung
    document.getElementById('DauSize_S').innerHTML = "&nbsp" + x.DauSize
    document.getElementById('Mau_S').innerHTML = "&nbsp" + x.TenMau;
    var drSize = dtBarCode.filter(y => y.Barcode == x.Barcode)[0];
    document.getElementById('Size_S').innerHTML = "&nbsp" + drSize.Size;
    // document.getElementById('SLScan_S').innerHTML = "&nbsp" + x.STDaQuet +'/'+ x.SLThung
}
function HandleTimeOut() {
    flagPrevent = false;
    clearTimeout(myTimeout);
}
function TestQR() {
    var audio = new Audio('/img/scannerAudio.mp3');
    audio.play();
    let qrCodeMessage = '1|MVT_NL_0003@MVT_NL_0003|1096#';
    var drCheckBarCode = dtData.filter(x => x.Barcode == qrCodeMessage);
    if (drCheckBarCode.length <= 0) {
        Swal.fire({
            title: `Vui lòng kiểm tra lại barcode đã được khai báo chưa!`,
            icon: 'warning',
            showConfirmButton: false,
            timer: 2500, // Thời gian hiển thị thông báo (1500 ms = 1.5 giây)
            timerProgressBar: true, // Hiển thị thanh tiến trình của timer
            allowOutsideClick: false,
            backdrop: true,
            didOpen: () => {
                Swal.showLoading(); // Hiển thị thanh tiến trình khi thông báo đang được hiển thị
            }
        });
        return;
    }
    var drCheckThung = dtData.filter(x => x.Barcode == qrCodeMessage && x.STDaQuet + 1 <= x.SLThung);
    if (drCheckThung.length <= 0) {
        Swal.fire({
            title: `SL Thùng đã vượt số lượng!`,
            icon: 'warning',
            showConfirmButton: false,
            timer: 2500, // Thời gian hiển thị thông báo (1500 ms = 1.5 giây)
            timerProgressBar: true, // Hiển thị thanh tiến trình của timer
            allowOutsideClick: false,
            backdrop: true,
            didOpen: () => {
                Swal.showLoading(); // Hiển thị thanh tiến trình khi thông báo đang được hiển thị
            }
        });
        BindingCardScan(drCheckBarCode[0]);
        return;
    }

    for (let i = 0; i < dtData.length; i++) {
        var x = dtData[i]
        if (x.Barcode.trim() == qrCodeMessage.trim()) {
            if ((x.STDaQuet + 1) <= x.SLThung) {
                x.STDaQuet = x.STDaQuet + 1;
                BindingCardScan(x);
                Save(x);
                break;
            }
        }
    }
    RenderTable(dtData);
}
function clearCamera() {
    html5QrCode
        .stop()
        .then((res) => {
            html5QrCode.clear();
        })
        .catch((err) => {
            console.log(err.message);
        });
    $("#scanQR").removeClass("active")
    $("#scanQR").text("Quét barcode")
}
function onScanSuccess(qrCodeMessage) {

}

function onScanError(errorMessage) {
    //handle scan error
}
