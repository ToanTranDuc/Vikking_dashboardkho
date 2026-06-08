var maPhieuXH = '';
var Cont = '';
var BarCodeCheck = '',KhachHang='';
var dtDataT=[],dtData = [],dtAlarm=[];
var lstScaned = [];
let html5QrCode = new Html5Qrcode("reader");
var dtBarCode = [];
var flagChangePXH = false;
var html5QrcodeScanner = new Html5QrcodeScanner(
    "reader", { fps: 10, qrbox: 250 });
var statusCreate = 0;
html5QrcodeScanner.render(onScanSuccess, onScanError);
const qrConfig = { fps: 10, qrbox: { width: 300, height: 300 } };
const brConfig = { fps: 10, qrbox: { width: 300, height: 150 } };
var flagPrevent = false;
var userName = '';
var flagLock = false;
var IsThungLe = 0;
var timeoutGetData;
var flagPreventRender = false;
$(document).ready(function () {
    userName = localStorage.getItem('username');
    html5QrcodeScanner.clear();
    GetKhachHang();
    timeoutGetData = setTimeout(GetData, 1000);
     //getPhieuXH();
})
$("#home").on("click", function () {
    window.location.href = '/Home/Dashboard'
})
function GetKhachHang() {
    $.ajax({
        url: `/api/XuatHang/Get?action=GetKhachHang&Para1=A&Para2=A&Para3=A`,
        type: 'GET',
        success: function (data) {
            $.each(data, function (key, items) {
                $('#KhachHang').append($('<option></option>').val(items.MaKH).html(items.TenKH));
            })
            //if (data.length > 0) $('#MaPKLXH').val(data[0].value);
            $('#KhachHang').val('KH_2');
            KhachHang ='KH_2'
            //PhieuXHChange();  
            getPhieuXH();
        }
    })
}
function KhachHangChange() {
    KhachHang = $('#KhachHang').val()
    getPhieuXH();
}
function getPhieuXH() {
    var fromdate = '2024-01-28';
    var todate = '2025-01-01'
    $.ajax({
        url: `/api/ScanBarcode/Get?action=GetPhieuXHScan&Para1=${KhachHang}&Para2=${todate}&Para3=A`,
        type: 'GET',
        success: function (data) {
            $('#MaPKLXH').html('');
           // data = data.filter(x => x.MaKH == $('#KhachHang').val());
            $.each(data, function (key, items) {
                $('#MaPKLXH').append($(`<option data-cont="${items.MaCont}"></option>`).val(items.MaPKL_XH).html(items.MaPKLXH_Display));
            })
            //if (data.length > 0) $('#MaPKLXH').val(data[0].value);
            $('#MaPKLXH').val('');
            //PhieuXHChange();            
        }
    })
}
function PhieuXHChange() {

   // flagLock = true;
    maPhieuXH = $('#MaPKLXH').val();
    Cont = $('#MaPKLXH option:selected').attr('data-cont');
    //GetBarcode();
    flagChangePXH = true;
    //GetData(1);    
    timeoutGetData = setTimeout(GetData, 1000);
}
function CheckThungLeChange() {
    IsThungLe = document.getElementById('cbxThungLe').checked == true ? 1 : 0;
    dtBarCode = dtData.filter(x => x.IsThungLe == IsThungLe);
    BingingDauSizeID();
}
//function HandleTimer() {
//    if (flagLock) {       
//        return;
//    }
//    GetData(0);
//}
function GetData() {
    $.ajax({
        url: `/api/ScanBarcode/GetDS?action=GetCTPhieuScan&Para1=${maPhieuXH}&Para2=${Cont}&Para3=${userName}`,
        type: 'GET',
        success: function (data) {
            clearTimeout(timeoutGetData);
            dtDataT = data.data1;//.filter(x => x.Cont == Cont);
            dtAlarm = data.data2;
            if (dtAlarm.length > 0) {
                AlarmScanBarcode();
            }
            dtBarCode = dtDataT.filter(x => x.IsThungLe == IsThungLe);
            if (dtDataT == null || dtDataT.length == 0) {
                timeoutGetData = setTimeout(GetData, 1000);
                return;
            };
            if (flagChangePXH && !flagPreventRender && maPhieuXH == dtDataT[0]["MaPKL_XH"]) {
                console.log("Đã vào " + maPhieuXH + " - SL: " + dtDataT.length);
                //dtBarCode = dtDataT.filter(x => x.IsThungLe == IsThungLe);
                console.log
                BingdingMaHang(dtDataT);
                flagChangePXH = false;
                
            }
            dtData = dtDataT.filter(x => x.valueMaHang == $('#MaHang').val() && x.POID == $('#PO').val());
            RenderTable(dtData);
            var dtSumBarcode = dtData.filter(x => x.valueMaHang == $('#MaHang').val() && x.POID == $('#PO').val() && x.DauSizeID == $('#DauSize').val()
                && x.ColorID == $('#Mau').val() && x.SizeID == $('#Size').val() && x.IsThungLe == IsThungLe);
            var sumSTDaQuet = dtSumBarcode.reduce((n, { STDaQuet }) => n + STDaQuet, 0);
            var sumSTKH = dtSumBarcode.reduce((n, { SLThung }) => n + SLThung, 0);
            document.getElementById('STTong_S').innerHTML = "&nbsp" + sumSTDaQuet + "/" + sumSTKH;
            timeoutGetData = setTimeout(GetData, 1000);
            //if (BarCodeCheck == "") {
               
            //    return;
            //}
            //else {
            //    var dtSumBarcode = dtData.filter(x => x.Barcode == BarCodeCheck);
            //    var sumSTDaQuet = dtSumBarcode.reduce((n, { STDaQuet }) => n + STDaQuet, 0);
            //    var sumSTKH = dtSumBarcode.reduce((n, { SLThung }) => n + SLThung, 0);
            //    document.getElementById('STTong_S').innerHTML = sumSTDaQuet + "/" + sumSTKH
            //}
          
        }
    })
}
function AlarmScanBarcode() {
    var dr = dtAlarm[0];    
    switch (dr.MaLoi) {
        case "1":
            Alert(dr.Content, 'success', 1000);
            break;
        case "2":
            var audio = new Audio('/img/audioError.mp3');
            audio.play();
            Alert(dr.Content, 'warning', 2000);
           
            break;
        case "3":
            var audio = new Audio('/img/audioError.mp3');
            audio.play();
            Alert(dr.Content, 'warning', 2000);
         
            break;
        case "4":
            var audio = new Audio('/img/audioError.mp3');
            audio.play();
            Alert(dr.Content, 'warning', 4000);

            break;
    }
}

function GetBarcode() {
    $.ajax({
        url: `/api/XuatHang/Get?action=GetBarcode&Para1=${maPhieuXH}&Para2=${Cont}&Para3=A`,
        type: 'GET',
        success: function (data) {
            dtBarCode = data;
            BingdingMaHang(data);
        }
    })
}
function BingdingMaHang() {
    $('#MaHang').html('');
    var lstMaHang = [...new Set(dtBarCode.map(item => item.valueMaHang))];
    $.each(lstMaHang, function (key, valueMaHang) {
        var lstdataMH = $.grep(dtBarCode, function (e) { return e.valueMaHang == valueMaHang })
        $('#MaHang').append($('<option></option>').val(valueMaHang).html(lstdataMH[0].MaHangDisplay));
    })
    MaHangChange();
}
function MaHangChange() {
    BingdingPO();
}
function BingdingPO() {
    $('#PO').html('');
    var lstdataMH = $.grep(dtBarCode, function (e) { return e.valueMaHang == $('#MaHang').val() })
    var lstPO = [...new Set(lstdataMH.map(item => item.POID))];
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
    var lstdataPO = $.grep(dtBarCode, function (e) { return e.valueMaHang == $('#MaHang').val() && e.POID == $('#PO').val() })
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
    var lstdata = $.grep(dtBarCode, function (e) { return e.valueMaHang == $('#MaHang').val() && e.POID == $('#PO').val() && e.DauSizeID == $('#DauSize').val() })
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
    var lstdata = $.grep(dtBarCode, function (e) { return e.valueMaHang == $('#MaHang').val() && e.POID == $('#PO').val() && e.DauSizeID == $('#DauSize').val() && e.ColorID == $('#Mau').val() })
    var lstSize = [...new Set(lstdata.map(item => item.SizeID))];
    $.each(lstSize, function (key, SizeID) {
        var lstdataSize = $.grep(lstdata, function (e) { return e.SizeID == SizeID })
        $('#Size').append($(`<option data-slgop =${lstdataSize[0].SLGop}></option>`).val(SizeID).html(lstdataSize[0].Size));
    })
    SizeChange();
   
}
function SizeChange() {
    var lstTemp = $.grep(dtBarCode, function (e) { return e.valueMaHang == $('#MaHang').val() && e.POID == $('#PO').val() && e.DauSizeID == $('#DauSize').val() && e.ColorID == $('#Mau').val() && e.SizeID == $('#Size').val() })
    BarCodeCheck = lstTemp[0].Barcode;
    BindingCardScanN();
    if (BarCodeCheck == "") {
        $('#ModalBarcode').modal('show');
    }
    else {
        statusCreate = 0;
        ConfirmSize();
    }   
}
function CancelCreate() {
    BarCodeCheck = "";
    statusCreate = 0;
    ConfirmSize();
}
function ConfirmCreate() {
    BarCodeCheck = "";
    statusCreate = 1;
    ConfirmSize();
}

function ConfirmSize() {
    flagPreventRender = true;
    var valMaHang = $('#MaHang').val().split('||');
    let ojSave = {       
        MaPKL_XH: maPhieuXH,
        MaHang: valMaHang[0],
        Season: valMaHang[1], //$("#MaHang option:selected").text().split('||')[1],
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
        IsCreate: statusCreate,
        IsThungLe: IsThungLe,
        SLGop: $('#Size option:selected').attr('data-slgop'),
        NVien: userName,

    }
    $.ajax({
        url: '/api/XuatHang/UpdateBarcodeLink?Action=UpdateBarcodeLink',
        type: 'Post',
        dataType: 'json',
        data: JSON.stringify(ojSave),
        contentType: 'application/json',
        success: function (data) {
            flagPreventRender = false;
        },
        error: function (err) {
            flagPreventRender = false;
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
    var sumSTDaQuet = dtData.reduce((n, { STDaQuet }) => n + STDaQuet, 0);
    var sumSTKH = dtData.reduce((n, { SLThung }) => n + SLThung, 0);
    var htmlHeader1 = '<th>Từ thùng</th><th>Đến thùng</th>';
    var htmlBody = '';   
    data.map((x, index) => {
        htmlBody += `<tr class="${x.IsThungLe == 1 ? "IsThungLe" : ""}" > <td>${x.TenHang}</td>
                         <td>${x.PO}</td>
                         <td>${x.DauSize}</td>
                         <td>${x.TenMau}</td>                        
                         <td>${x.Size}</td>
                         <td>${x.SLThung}</td>
                         <td>${x.STDaQuet}</td>
                         <td>${x.Barcode}</td></tr>`
       
    });
    //<td style="font-family: 'Libre Barcode 39 Extended Text'; font-size:15px;">${x.Barcode}</td>
    htmlBody += `<tr class="sumrow"><td colspan=5>Tổng</td>
                      <td>${sumSTKH}</td>
                      <td>${sumSTDaQuet}</td>
 <td></td>
                    </tr>`
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
    document.getElementById('MaHang_S').innerHTML = "&nbsp" + $("#MaHang option:selected").text().split('||')[0]
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
function Alert(title,icon,time) {
    Swal.fire({
        title: title,
        icon: icon,
        showConfirmButton: false,
        timer: time, // Thời gian hiển thị thông báo (1500 ms = 1.5 giây)
        timerProgressBar: true, // Hiển thị thanh tiến trình của timer
        allowOutsideClick: false,
        backdrop: true,
        didOpen: () => {
            Swal.showLoading(); // Hiển thị thanh tiến trình khi thông báo đang được hiển thị
        }
    });

}
