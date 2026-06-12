

let html5QrCode;
let lastScanned = null;
let scanCooldown = false;
let isProcessing = false;
function PlayAudio() {
    const beepSound = document.getElementById("beepSound");
    beepSound.currentTime = 0;
    beepSound.play();
}
function PlayAudioError() {
    const beepSoundE = document.getElementById("beepSoundError");
    beepSoundE.currentTime = 0;
    beepSoundE.play();
}
// Bắt đầu quét
let lastScanTime = 0;

function startScan() {
    $("#reader").addClass("active")
    document.getElementById('reader').style.display = 'block';
    document.getElementById('shaded-region').style.display = 'block';

    html5QrCode = new Html5Qrcode("reader");

    html5QrCode.start(
        { facingMode: "environment" },
        {
            fps: 30,
            qrbox: { width: 300, height: 300 }
        },
        (decodedText, decodedResult) => {
            const now = Date.now();
            const timeSinceLastScan = now - lastScanTime;

            if (timeSinceLastScan < 2000) {
                return; // ❌ Chưa đủ 2 giây, bỏ qua lần quét này
            }

            lastScanTime = now; // ✅ Cập nhật mốc thời gian lần quét


            CheckBarCode(decodedText)




        },
        (errorMessage) => {
            // Có thể log ra console nếu cần debug
        }
    ).catch((err) => {
        console.error("Lỗi khi bật camera:", err);
        alert("Không thể mở camera: " + err);
    });
}


// Dừng quét
function stopScan() {
    if (html5QrCode) {
        html5QrCode.stop().then(() => {
            html5QrCode.clear();
            $("#reader").removeClass("active")
            document.getElementById('reader').style.display = 'none';
            document.getElementById('shaded-region').style.display = 'none';  // Ẩn vùng tối

        }).catch(err => {
            console.error("Lỗi khi dừng camera:", err);
        });
    }
}
let scannedCode = '';
$(document).on('keypress', function (e) {
    //const char = String.fromCharCode(e.which);

    //if (e.which !== 13) {
    //    scannedCode += char;
    //} else {
    //    //if ($('#QRScan:focus').length > 0) {
    //        CheckBarCode(scannedCode);
    //    //}
    //    //else {
    //    //    iziToast.warning({
    //    //        title: 'Warning',
    //    //        message: 'Vui lòng chọn vào ô QR scan.',
    //    //        position: 'topRight'

    //    //    });
    //    //}
    //    scannedCode = ''; // reset để lần sau quét mới
    //}
});
$(document).on('keydown', e => {

    if (e.key === 'Enter') {
        if ($('#QRScan:focus').length > 0) {
            CheckBarCode(e.target.value.trim());
        }
        else {
            iziToast.warning({
                title: 'Warning',
                message: 'Vui lòng chọn vào ô QR scan.',
                position: 'topRight'

            });
        }


    }
});
var dataDHPO = []
async function GetDHPO() {
    var url = `/api/ScanBarcodeDT_NK/Get?Action=GetDH`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        dataDHPO = data.Table
        await GetDH()

    } catch (error) {
        console.error(error.message);
    }
}
async function GetDH() {
    var dataDH = [
        ...new Map(dataDHPO.map((item) =>
            [`${item.valueDisplay}-${item.MaGop}-${item.MaHang}`, { valueDisplay: item.valueDisplay, MaGop: item.MaGop, MaHang: item.MaHang }]
        )).values()
    ];
    let html = ""
    dataDH.map(x => {
        html += `<option data-mahang="${x.MaHang}" value="${x.MaGop}">${x.valueDisplay}</option> `
    })
    $("#MaDH").html(html)
    await GetPO()
}
async function GetPO() {
    var dataPO = dataDHPO.filter(x => x.MaGop == $("#MaDH").val())
    dataPO = [
        ...new Map(dataPO.map((item) =>
            [`${item.POID}-${item.PO}`, { POID: item.POID, PO: item.PO }]
        )).values()
    ];
    let html = ""
    dataPO.map(x => {
        html += `<option value="${x.POID}">${x.PO}</option> `
    })
    $("#POID").html(html)
    GetThongTinDT()
}
var barcode = []
var barcodeN = []
var barCodeThungGop = []
async function GetThongTinDT() {
    var url = `/api/ScanBarcodeDT_NK/Get?Action=GetCTDongThung&para1=${$("#MaDH").val()}&para2=${$("#POID").val()}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        barcode = data.Table
        barcodeN = data.Table1
        barCodeThungGop = data.Table.filter(x => x.SLGop > 1)
        LoadBody(data.Table)

    } catch (error) {
        console.error(error.message);
    }
}
$(function () {
    GetDHPO()
})
function GetDauSize() {
    var dataDS = barCodeThungGop.filter(x => x.MaDH == $("#MaDH").val() && x.POID == $("#POID").val())
    dataDS = [
        ...new Map(dataDS.map((item) =>
            [`${item.DauSizeID}-${item.DauSize}`, { DauSizeID: item.DauSizeID, DauSize: item.DauSize }]
        )).values()
    ];
    let html = ""
    dataDS.map(x => {
        html += `<option value="${x.DauSizeID}">${x.DauSize}</option> `
    })
    $("#DauSize").html(html)
    GetMau()
}
function GetMau() {
    var dataM = barCodeThungGop.filter(x => x.MaDH == $("#MaDH").val() && x.POID == $("#POID").val() && x.DauSizeID == $("#DauSize").val())
    dataM = [
        ...new Map(dataM.map((item) =>
            [`${item.ColorID}-${item.TenMau}`, { ColorID: item.ColorID, TenMau: item.TenMau }]
        )).values()
    ];
    let html = ""
    dataM.map(x => {
        html += `<option value="${x.ColorID}">${x.TenMau}</option> `
    })
    $("#Mau").html(html)
    GetSize()
}

function GetSize() {
    arrQRScan = []
    var dataS = barCodeThungGop.filter(x => x.MaDH == $("#MaDH").val() && x.POID == $("#POID").val() && x.DauSizeID == $("#DauSize").val() && x.ColorID == $("#Mau").val())

    dataS = [
        ...new Map(dataS.map((item) =>
            [`${item.SizeID}-${item.Size}`, { SizeID: item.SizeID, Size: item.Size }]
        )).values()
    ];
    let html = ""
    dataS.map(x => {
        html += `<option value="${x.SizeID}">${x.Size}</option> `
    })
    $("#Size").html(html)
}
function LoadBody(data) {
    let html = ""
    data.map(x => {
        html += `
            <tr style="background: ${x.IsThungLe ? "#FFD480 " : ""} ">
             <td>${x.TenHang}</td>
             <td>${x.PO}</td>
             <td>${x.DauSize}</td>
             <td>${x.TenMau}</td>
             <td>${x.Size}</td>
             <td>${x.SLThung}</td>
             <td>${x.STDaDong || ""}</td>
             <td style="max-width:300px;min-width:150px">${x.Barcode || ""}</td>
            </tr>

            `
    })
    $("#tbody").html(html)
    $("#QRScan").focus()
}
var arrQRScan = []
async function CheckBarCode(barcodeValue) {
    if ($("#cbxThungLe").is(":checked")) {
        //if (arrQRScan.includes(barcodeValue)) {
        //    return;
        //}
        arrQRScan.push(barcodeValue);
        var dataGopBC = barCodeThungGop.filter(x => x.MaDH == $("#MaDH").val() && x.POID == $("#POID").val() && x.DauSizeID == $("#DauSize").val() && x.ColorID == $("#Mau").val() && x.SizeID == $("#Size").val())
        var d = dataGopBC[0]
        if (d.STDaDong == 1) {
            iziToast.warning({
                title: 'Warning',
                message: 'Thùng đã được đóng.',
                position: 'topRight'

            });
            PlayAudioError()
            $("#QRScan").val("")
            return
        }
        const arrJoin = arrQRScan.join(",")
        $("#QRScan").val(arrJoin)
        const barcodeSplit = d.Barcode
        if (barcodeSplit == null) {
            iziToast.warning({
                title: 'Warning',
                message: 'Barcode chưa được khai báo.',
                position: 'topRight'

            });
            PlayAudioError()
            $("#QRScan").val("")
            return
        }
        const barcodeLength = barcodeSplit.split(",")
        if (arrQRScan.length == barcodeLength.length) {
            if (!areStringSetsEqual(arrJoin, barcodeSplit)) {
                iziToast.warning({
                    title: 'Warning',
                    message: 'Barcode không trùng khớp trong danh sách đã khai báo .',
                    position: 'topRight'
                });
                arrQRScan = []
                PlayAudioError()
                $("#QRScan").val("")
                return
            }
            arrQRScan = []
            $("#QRScan").val("")
            $(".hightlight").text(`1/1`)

            for (let x of barCodeThungGop) {
                if (x.MaDH == d.MaDH && x.POID == d.POID && x.DauSizeID == d.DauSizeID && x.ColorID == d.ColorID && x.SizeID == d.SizeID
                    && x.SttThung == d.SttThung
                ) {
                    x.STDaDong = 1;
                    break;
                }
            }
            for (let x of barcode) {
                if (x.MaDH == d.MaDH && x.POID == d.POID && x.DauSizeID == d.DauSizeID && x.ColorID == d.ColorID && x.SizeID == d.SizeID && x.IsThungLe == d.IsThungLe
                ) {
                    x.STDaDong = 1;
                    break;
                }
            }
            const dataBarCodeN = barcodeN.filter(x => x.MaDH == d.MaDH && x.POID == d.POID && x.DauSizeID == d.DauSizeID && x.ColorID == d.ColorID && x.SizeID == d.SizeID && x.IsThungLe == d.IsThungLe && x.SttThung == d.SttThung)


            await LoadBody(barcode)
            var arrScanThung = []
            let object = {
                MaDH: d.MaDH,
                POID: d.POID,
                MaPKL: dataBarCodeN[0].MaPKL,
                SttThung: d.SttThung,
                NgayNhapKho_TC: ""
            }
            arrScanThung.push(object)
            PostSave(arrScanThung)

        }
    }
    else {
        let itemCheckBarCode = barcode.find(item => item.Barcode == barcodeValue);
        let itemCheckBarCodeN = barcodeN.find(item => item.Barcode == barcodeValue);

        if (!itemCheckBarCode && !itemCheckBarCodeN) {
            iziToast.warning({
                title: 'Warning',
                message: 'Barcode chưa được khai báo.',
                position: 'topRight'

            });
            PlayAudioError()
            $("#QRScan").val("")
            return
        }
        if (itemCheckBarCode) {
            var d = itemCheckBarCode
            const madh = d.MaDH
            const poid = d.POID
            const dausizeid = d.DauSizeID
            const colorid = d.ColorID
            const sizeid = d.SizeID
            const isThungLe = d.IsThungLe
            $("#MaHang_S").text(d.TenHang)
            $("#PO_S").text(d.PO)
            $("#DauSize_S").text(d.DauSize)
            $("#Mau_S").text(d.TenMau)
            $("#Size_S").text(d.Size)


            const dataBarCodeN = barcodeN.filter(x => x.MaDH == madh && x.POID == poid && x.DauSizeID == dausizeid && x.ColorID == colorid && x.SizeID == sizeid && x.IsThungLe == isThungLe)

            const dataCDT = dataBarCodeN.filter(x => x.IsDongThung != 1)
            if (dataCDT.length == 0) {
                iziToast.warning({
                    title: 'Warning',
                    message: 'Thùng đã được đóng hết.',
                    position: 'topRight'
                });
                PlayAudioError()
                $(".hightlight").text(`${dataBarCodeN.length}/${dataBarCodeN.length}`)
                $("#QRScan").val("")
                return
            };
            const minItem = dataCDT.reduce((min, current) => {
                return current.SttThung < min.SttThung ? current : min;
            });
            for (let x of barcodeN) {
                if (x.MaDH == minItem.MaDH && x.POID == minItem.POID && x.DauSizeID == minItem.DauSizeID && x.ColorID == minItem.ColorID && x.SizeID == minItem.SizeID && x.IsThungLe == minItem.IsThungLe
                    && x.SttThung == minItem.SttThung
                ) {
                    x.IsDongThung = 1;
                    break;
                }
            }

            for (let x of barcode) {
                if (x.MaDH == minItem.MaDH && x.POID == minItem.POID && x.DauSizeID == minItem.DauSizeID && x.ColorID == minItem.ColorID && x.SizeID == minItem.SizeID && x.IsThungLe == minItem.IsThungLe
                ) {
                    x.STDaDong = x.STDaDong + 1;
                    break;
                }
            }

            $(".hightlight").text(`${dataBarCodeN.length - dataCDT.length + 1}/${dataBarCodeN.length}`)
            await LoadBody(barcode)
            var arrScanThung = []
            let object = {
                MaDH: madh,
                POID: poid,
                MaPKL: minItem.MaPKL,
                SttThung: minItem.SttThung,
                NgayNhapKho_TC: ""
            }
            arrScanThung.push(object)
            PostSave(arrScanThung)

            return
        }
        if (itemCheckBarCodeN) {

            var d = itemCheckBarCodeN
            const madh = d.MaDH
            const poid = d.POID
            const dausizeid = d.DauSizeID
            const colorid = d.ColorID
            const sizeid = d.SizeID
            const isThungLe = d.IsThungLe
            const MaPKL = d.MaPKL
            $("#MaHang_S").text($("#MaDH option:selected").data("mahang"))
            $("#PO_S").text(d.PO)
            $("#DauSize_S").text(d.DauSize)
            $("#Mau_S").text(d.TenMau)
            $("#Size_S").text(d.Size)
            const dataBarCodeN = barcodeN.filter(x => x.MaDH == madh && x.POID == poid && x.DauSizeID == dausizeid && x.ColorID == colorid && x.SizeID == sizeid && x.IsThungLe == isThungLe)

            const dataCDT = dataBarCodeN.filter(x => x.IsDongThung != 1)

            if (d.IsDongThung == 1) {
                iziToast.warning({
                    title: 'Warning',
                    message: 'Thùng đã được đóng.',
                    position: 'topRight'
                });
                PlayAudioError()
                $(".hightlight").text(`${dataBarCodeN.length - dataCDT.length}/${dataBarCodeN.length}`)
                return
            }
            $(".hightlight").text(`${dataBarCodeN.length - dataCDT.length + 1}/${dataBarCodeN.length}`)



            for (let x of barcodeN) {
                if (x.MaDH == d.MaDH && x.POID == d.POID && x.DauSizeID == d.DauSizeID && x.ColorID == d.ColorID && x.SizeID == d.SizeID && x.IsThungLe == d.IsThungLe
                    && x.SttThung == d.SttThung
                ) {
                    x.IsDongThung = 1;
                    break;
                }
            }

            for (let x of barcode) {
                if (x.MaDH == d.MaDH && x.POID == d.POID && x.DauSizeID == d.DauSizeID && x.ColorID == d.ColorID && x.SizeID == d.SizeID && x.IsThungLe == d.IsThungLe
                ) {
                    x.STDaDong = x.STDaDong + 1;
                    break;
                }
            }

            await LoadBody(barcode)
            var arrScanThung = []
            let object = {
                MaDH: madh,
                POID: poid,
                MaPKL: MaPKL,
                SttThung: d.SttThung,
                NgayNhapKho_TC: ""
            }
            arrScanThung.push(object)
            PostSave(arrScanThung)
        }
    }
}

const PostSave = async (arrPost) => {
    PlayAudio();
    const request = new Request(`/api/ScanBarcodeDT_NK/PostWeb?action=PostDT`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrPost),
    }
    );
    let response = await fetch(request);
    let data = await response.json();
    iziToast.success({
        title: "Success",
        message: `Lưu thành công`,
        position: "topRight",
        timeout: 500
    });

    $("#QRScan").val("")
};
function CheckThungLeChange() {
    if ($("#cbxThungLe").is(":checked")) {
        GetDauSize()
        GetText()
        $("#QRScan").focus()
    }
    else {
        $("#DauSize").empty()
        $("#Mau").empty()
        $("#Size").empty()
        $("#QRScan").val("")

        $("#MaHang_S").text("")
        $("#PO_S").text("")
        $("#DauSize_S").text("")
        $("#Mau_S").text("")
        $("#Size_S").text("")
        $(".hightlight").text("")
    }

}
function GetText() {
    $("#QRScan").focus()
    arrQRScan = []
    var dataGopBC = barCodeThungGop.filter(x => x.MaDH == $("#MaDH").val() && x.POID == $("#POID").val() && x.DauSizeID == $("#DauSize").val() && x.ColorID == $("#Mau").val() && x.SizeID == $("#Size").val())
    console.log(dataGopBC)
    const d = dataGopBC[0]

    $("#MaHang_S").text($("#MaDH option:selected").data("mahang"))
    $("#PO_S").text(d.PO)
    $("#DauSize_S").text(d.DauSize)
    $("#Mau_S").text(d.TenMau)
    $("#Size_S").text(d.Size)
    const isDT = d.STDaDong > 0 ? 1 : 0
    $(".hightlight").text(`${isDT}/${1}`)
}
function areStringSetsEqual(str1, str2) {
    const arr1 = str1.split(',').sort();
    const arr2 = str2.split(',').sort();
    return arr1.join(',') === arr2.join(',');
}
$("#home").on("click", function () {
    window.location.href = '/Home/DashBoard'
})