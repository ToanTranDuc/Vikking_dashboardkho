
var batchData;
var dataKiem;
var dataErrorPoint;
// Khởi tạo
$(document).ready(function () {

    GetSoLoID()
});

async function GetSoLoID() {
    const url = `/api/KiemVai/Get?action=GetBCSoLo`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        let html = `<option value=""></option>`;
        data.dt1.map(x => {
            html += `
                <option data-ncc="${x.NCC}" data-khachhang="${x.TenKH}"  value="${x.SoLoID}">${x.SoLo} - ${x.NgayNhap} - PO Mua: ${x.POMua}</option>
            `
        })
        $("#SoLo").html(html)
        $("#SoLo").val("")
        $("#minFabricAlert").attr("style", "display: none !important;");
    } catch (error) {
        console.error(error.message);
    }
}

async function GetMauVai() {
    $("#minFabricAlert").attr("style", "display: none !important;");
    $("#batchInput").val("");
    selectedItems = [];
    const loaivai = $("#PhuLieuA option:selected").text()
    $(".text-loaivai").text(loaivai)
    const url = `/api/KiemVai/Get?action=GetBCMauVai&para1=${$("#SoLo").val()}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);
        let html = ``;
        const data = await response.json();
        data.dt1.map(x => {
            html += `
                <option data-mauvt="${x.MauVTID}" data-mavtid="${x.MaVTID}" value="${x.MaNPL}">${x.MaVT} - ${x.MauVT}</option>
            `
        })
        $("#MauVai").html(html)
        GetBatchLot()
    } catch (error) {
        console.error(error.message);
    }
}
async function GetBatchLot() {
    selectedItems = [];

    const mauVTID = $("#MauVai option:selected").data("mauvt")
    const url = `/api/KiemVai/Get?action=GetBCBatchLot&para1=${$("#SoLo").val()}&para2=${$("#MauVai").val()}&para3=a`;

    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        batchData = data.dt1;

        selectedItems = batchData.map(item => ({
            batch: item.Batch,
            lot: item.SoLoT,
            socay: item.SoCay,
            manpl: item.MaNPL
        }));

        updateInput();

        checkLowInspectionRatio(batchData);
        checkAllMinFabricByAPI(batchData, mauVTID);
        renderList(data.dt1);
    } catch (error) {
        console.error(error.message);
    }

    //$("#batchInput").val("");
    $("#checkAll").prop("checked", true);

    // Xóa bảng kết quả cũ nếu có
    $('.inspection-table').empty();
    $(".divResult").addClass("d-none");
    GetBCKiemVai()
}
let selectedItems = [];

// Render danh sách
function renderList(data) {
    const itemList = $('#itemList');
    itemList.empty();
    data.forEach(item => {
        const html = `
            <div class="dropdown-item">
                <input type="checkbox" 
                       id="${item.SoLoT}${item.Batch}${item.SoCay}"
                       data-batch="${item.Batch}"
                       data-lot="${item.SoLoT}"
                       data-socay="${item.SoCay}"
                       checked disabled> 
                <label for="${item.SoLoT}${item.Batch}${item.SoCay}">${item.Display}</label>
            </div>
        `;
        itemList.append(html);
    });
    $('#checkAll').prop('checked', true).prop('disabled', true);
}

// Cập nhật input - hiển thị cả Batch và LOT
function updateInput() {
    const displayText = selectedItems.map(item => `${item.batch}/${item.lot}/${item.socay}`).join(', ');
    $('#batchInput').val(displayText);
}

// Cập nhật trạng thái check all
function updateCheckAll() {
    const total = $('#itemList input[type="checkbox"]').length;
    const checked = $('#itemList input[type="checkbox"]:checked').length;
    $('#checkAll').prop('checked', total > 0 && total == checked);
}

// Click vào input
$('#batchInput').click(function (e) {
    e.stopPropagation();
    $('#dropdownList').toggleClass('show');
    $('#searchInput').val('').focus();
    renderList(batchData);
});

//Click ẩn notification
$("#btn-hide").click(function (e) {
    e.stopPropagation();
    $("#minFabricAlert").attr("style", "display: none !important;");
    updateTableHeight();
});

$("#btn-hide-1").click(function (e) {
    e.stopPropagation();
    $("#percentInput").attr("style", "display: none !important;");
    updateTableHeight();
});

//// Xử lý checkbox
//$(document).on('change', '#itemList input[type="checkbox"]',async function () {
//    const batch = $(this).data('batch');
//    const lot = $(this).data('lot');
//    const socay = $(this).data('socay');
    
//    if ($(this).is(':checked')) {
//        // Thêm nếu chưa có
//        if (!selectedItems.some(item => item.batch == batch && item.lot == lot && item.socay == socay)) {
//            selectedItems.push({ batch: batch, lot: lot, socay: socay});
//        }
//    } else {
//        // Xóa khỏi mảng
//        selectedItems = selectedItems.filter(item => !(item.batch == batch && item.lot == lot && item.socay == socay));
//    }

//    //const invalidList = await checkMinFabricQuantity(selectedItems);
//    //displayFabricAlert(invalidList);

//    updateInput();
//    updateCheckAll();
//    GetBCKiemVai()
//});

//// Check all
//$('#checkAll').change(async function () {
//    const isChecked = $(this).is(':checked');

//    if (isChecked) {
//        selectedItems = batchData.map(item => ({
//            batch: item.Batch,
//            lot: item.SoLoT,
//            socay: item.SoCay
//        }));
//    } else {
//        selectedItems = [];
//    }

//    $('#itemList input[type="checkbox"]').prop('checked', isChecked);

//    //const invalidList = await checkMinFabricQuantity(selectedItems);
//    //displayFabricAlert(invalidList);

//    updateInput();
//    GetBCKiemVai()
//});

// Chỉnh sửa sự kiện change của checkbox
$(document).on('change', '#itemList input[type="checkbox"]', async function () {
    // Nếu bạn muốn khóa hoàn toàn, có thể không làm gì ở đây hoặc force check lại
    $(this).prop('checked', true);
    return false;
});

// Chỉnh sửa sự kiện change của Check All
$('#checkAll').change(function () {
    $(this).prop('checked', true);
});

// Tìm kiếm
$('#searchInput').on('keyup', function () {
    const searchText = $(this).val().toLowerCase();
    const filtered = batchData.filter(item =>
        item.Batch.toLowerCase().includes(searchText) ||
        item.SoLoT.toLowerCase().includes(searchText) ||
        item.SoCay.toLowerCase().includes(searchText) ||
        item.Display.toLowerCase().includes(searchText)
    );
    renderList(filtered);
});

// Đóng dropdown khi click ra ngoài
$(document).click(function () {
    $('#dropdownList').removeClass('show');
});

$('#dropdownList').click(function (e) {
    e.stopPropagation();
});

// Hàm lấy danh sách Batch và LOT đã chọn
function getSelectedBatchAndLot() {
    return {
        batches: selectedItems.map(item => item.batch),  // Mảng chỉ chứa Batch
        lots: selectedItems.map(item => item.lot),        // Mảng chỉ chứa LOT
        socays: selectedItems.map(item => item.socay),        // Mảng chỉ chứa LOT
        full: selectedItems                                // Mảng đầy đủ {batch, lot}
    };
}

let maNPL = null;

async function GetBCKiemVai() {
    const batchjoin = selectedItems.map(item => item.batch).join(";")
    const lotjoin = selectedItems.map(item => item.lot).join(";")
    const socay = selectedItems.map(item => item.socay).join(";")
    const mauVTID = $("#MauVai option:selected").data("mauvt")

    const url = `/api/KiemVai/Get?action=GetBCKiemVaiV2&para1=${$("#SoLo").val()}&para2=${$("#MauVai").val()}&para3=${mauVTID}&para4=${batchjoin}&para5=${lotjoin}&para6=${socay}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        maNPL = data.dt1[0].MaNPL
        if (data.dt1 && data.dt1.length > 0) {
            dataKiem = data.dt1
            dataErrorPoint = data.dt2
            createDynamicTable(data.dt1, data.dt2);
            $(".text-result").val(data.dt1[0].ResultQC);
            //$("#resultQC-pass").prop("checked", data.dt1[0].StatusResultQC == 1);
            //$("#resultQC-fail").prop("checked", data.dt1[0].StatusResultQC == 0);
            $(".divResult").removeClass("d-none")

            if (data.dt1[0].Sign != null && data.dt1[0].Sign != "") {
                console.log(data.dt1[0].Sign)
                $(".image1").attr("src", `/Images/SignKiemVai/${data.dt1[0].Sign}`);
            
            } 
            if (data.dt1[0].SignManager != null && data.dt1[0].SignManager != "") {
                $(".image2").attr("src",  `/Images/SignKiemVai/${data.dt1[0].SignManager}`);
            }
            if (data.dt1[0].SignReceive != null && data.dt1[0].SignReceive != "") {
                $(".image3").attr("src", `/Images/SignKiemVai/${data.dt1[0].SignReceive}`);
            }
        } else {
            $('.inspection-table').empty();
            $(".divResult").addClass("d-none")
        }
    } catch (error) {
        console.error(error.message);
    }
}
function createDynamicTable(dataArray, dtThongSoloi) {
    if (!dataArray || dataArray.length === 0) {
        return;
    }

    // Mapping defect types
    const defectMapping = {
        'Wrap bar': { vi: 'Lỗi sợi dọc', en: 'Wrap bar' },
        'Weft bar': { vi: 'Lỗi sợi ngang', en: 'Weft bar' },
        'Holes': { vi: 'Thủng lỗ', en: 'Holes', note: '(2đ ≥ 2,5cm,4đ > 2,5cm)' },
        'Oil stain/spot': { vi: 'Vết dầu/bẩn', en: 'Oil stain/spot' },
        'Color yarn': { vi: 'Sợi màu', en: 'Color yarn' },
        'Crease mark': { vi: 'Nếp gấp', en: 'Crease mark' },
        'Other defects': { vi: 'Lỗi khác', en: 'Other defects' }
    };

    // Cấu trúc các dòng kiểm tra
    const inspectionRows = [
        {
            stt: 1,
            label: "Mã số vải của xí nghiệp/NCC/",
            labelEn: "Fabric code",
            type: "static",
            field: "MaVT"
        },
        {
            stt: 2,
            label: "Màu của cty/NCC/",
            labelEn: "Color code of Viking & Supplier",
            type: "radio-text",
            radioName: "color",
            statusField: "ColorStatus",
            valueField: "ColorText",
            dataImage: `ColorImg`,
            datafiled: "color",
        },
        {
            stt: 3,
            label: "Thành phần % vải /",
            labelEn: "Percentage of error components",
            type: "radio-text",
            radioName: "percent",
            statusField: "PercentStatus",
            valueField: "PercentText",
            dataImage: `PercentImg`,
            datafiled: "percent",
        },
        {
            stt: 4,
            label: "Số Batch/LOT",
            labelEn: "Batch/LOT number",
            type: "static",
            field: (item) => `${item.Batch || ''} / ${item.LOT || ''} / ${item.SoCay || ''} `
        },
        {
            stt: 5,
            label: "Trọng lượng-(ghi rõ %)",
            labelEn: "Pass/fail weight",
            type: "radio-input-only",
            radioName: "weight",
            statusField: "TrongLuong",
            valueField: "Weight",
            dataImage: `ImgWeight`,
            datafiled:"weight",
        },
        {
            stt: 6,
            label: "Độ co rút-(ghi rõ %)",
            labelEn: "Pass/fail Shrinkage",
            type: "radio-input-only",
            radioName: "shrinkage",
            statusField: "DoCoRut",
            valueField: "Shrinkage",
            dataImage: `ImgShrinkage`,
            datafiled: "shrinkage",

        },
        {
            stt: 7,
            label: "Chống thấm",
            labelEn: "Pass/fail Waterproof",
            type: "radio-input-only",
            radioName: "waterproof",
            statusField: "ChongTham",
            valueField: "Waterproof",
            dataImage: `ImgWaterproof`,
            datafiled: "waterproof",
        },
        {
            stt: 8,
            label: "Số lượng vải nhập",
            labelEn: "Receiving quantity",
            type: "textarea",
            field: "ReceivingQuantity",
            unit: "TenDVVT"
        },
        {
            stt: 8,
            label: "Số lượng vải kiểm",
            labelEn: "Checking quantity",
            type: "textarea",
            field: "SLTT",
            unit: "TenDVVT"
        },
        {
            stt: 9,
            label: "Kiểm mặt phải",
            labelEn: "Face side",
            type: "radio-text",
            radioName: "checkFace",
            statusField: "StatusFaceSide",
            valueField: "FaceSide",
            dataImage: `ImgFaceSide`,
            datafiled: "checkFace",

        },
        {
            stt: 9,
            label: "Kiểm mặt trái",
            labelEn: "Back side",
            type: "radio-text",
            radioName: "checkBlack",
            statusField: "StatusBlackSide",
            valueField: "BlackSide",
            dataImage: `ImgBlackSide`,
            datafiled: "checkBlack",
        },
        {
            stt: 10,
            label: "Chiều dài/trọng lượng/khổ cây vải trên tem",
            labelEn: "Length/ Weight/ width roll in label",
            type: "textarea",
            field1: "RollLabel",
            field2: "KhoVai",
            unit1: "TenDVVT",
            unit2: "DonViKhoVai"
        },
        {
            stt: 10,
            label: "Chiều dài/trọng lượng/khổ thực tế",
            labelEn: "Length/ Weight/ width roll in actual",
            type: "textarea-with-radio",
            field1: "RollActual",
            field2: "KhoVaiActual",
            unit1: "TenDVVT",
            unit2: "DonViKhoVai",
            statusField1: "RollStatus",
            statusField2: "KhoVaiStatus"
        },
        {
            stt: 11,
            label: "Đệm bố/ CUT PROTECT",
            labelEn: "Thread direction / Vertical / Horizontal",
            type: "dual-textarea",
            field1: "CutProctect",
            field2: "ColorThread",
            label1: "Màu chỉ",
            label1En: "Color thread"
        },
        {
            stt: 12,
            label: "Loang màu",
            labelEn: "Color shading",
            type: "radio-text",
            radioName: "checkloang",
            statusField: "StatusColorShading",
            valueField: "ColorShading",
            dataImage: `ImgColorShading`,
            datafiled: "checkloang",
        },
        {
            stt: 13,
            label: "Điểm nối trên cây:",
            labelEn: "Joining points in roll",
            type: "textarea",
            field: "JoiningPointsRoll",
            hideUnit: true
        },
        
        {
            stt: 14,
            type: "defect-header"
        }
    ];

    // Tạo HTML cho header
    let headerHTML = `
        <thead style="position: sticky; top: -1px; z-index: 10;">
            <tr>
                <th style="width: 50px; text-align: center;">STT</th>
                <th style="min-width:400px">Nội dung kiểm tra</th>`;

    // Thêm cột cho mỗi loại vải
    dataArray.forEach((item, index) => {
        const fabricName = item.TenLoaiVai || `Vải ${index + 1}`;
        headerHTML += `<th colspan="4" style="width: ${70 / dataArray.length}%;">${item.Batch ??""} / ${item.LOT ??""} / ${item.SoCay}</th>`;
    });

    headerHTML += `</tr></thead>`;

    // Tạo HTML cho body
    let bodyHTML = '<tbody>';

    inspectionRows.forEach((row, rowIndex) => {

        bodyHTML += `<tr> <td class="row-number">${row.stt}</td>`;

        // Xử lý dòng STT 14 - Header cho defect table
        if (row.stt === 14) {
            bodyHTML += `<td class="text-left">
                <strong>Theo tiêu chuẩn 4 điểm</strong>/4 points system<br>
                <strong>1 điểm:</strong> (≤ 7.5cm), <strong>2 điểm:</strong> (7.6 ≥ 14.9cm)<br>
                <strong>3 điểm:</strong> (15 -> 23cm), <strong>4 điểm:</strong> ( > 23cm)
            </td>`;

            // Thêm header 1,2,3,4 cho mỗi loại vải
            dataArray.forEach((item, colIndex) => {
                bodyHTML += `
                    <td style="text-align: center; font-weight: bold;">1</td>
                    <td style="text-align: center; font-weight: bold;">2</td>
                    <td style="text-align: center; font-weight: bold;">3</td>
                    <td style="text-align: center; font-weight: bold;">4</td>
                `;
            });
        }

        
        // Xử lý dòng STT 10 - Đặc biệt
        else if (row.stt === 11) {
            bodyHTML += `<td class="label-col">
                <div class="row">
                    <div class="col-7" style="border-right: 1px solid #ccc;">
                        <div><span class="label-text">Đệm bố/ CUT PROTECT :</span></div>
                        <div class="sub-label">
                            - Hướng của sợi chỉ màu /
                            <span class="label-english">Thread direction</span>
                        </div>
                        <div class="sub-label">
                            - Dọc theo chiều đã cây vải /
                            <span class="label-english">Vertical</span>
                        </div>
                        <div class="sub-label">
                            - Ngang theo khổ vải /
                            <span class="label-english">Horizontal</span>
                        </div>
                    </div>
                    <div class="col-5 d-flex justify-content-center flex-column">
                        <div><span class="">Màu chỉ</span></div>
                        <div class="sub-label">
                            <span class="label-english">Color thread</span>
                        </div>
                    </div>
                </div>
            </td>`;

            // Xử lý giá trị cho dual-textarea
            dataArray.forEach((item, colIndex) => {
                bodyHTML += '<td style="min-width: 350px;" colspan="4" class="value-col">';
                const val1 = item[row.field1] || '';
                const val2 = item[row.field2] || '';
                bodyHTML += `
                    <div class="row">
                        <div class="col-6" style="border-right: 1px solid #ccc;">
                            <textarea style="color: #FF3A00" class="form-control" 
                                data-field="${row.field1}" data-col="${colIndex}" 
                                rows="2" readonly>${val1}</textarea>
                        </div>
                        <div class="col-6">
                            <textarea style="color: #FF3A00" class="form-control" 
                                data-field="${row.field2}" data-col="${colIndex}" 
                                rows="2" readonly>${val2}</textarea>
                        </div>
                    </div>`;
                bodyHTML += '</td>';
            });
        }
        else if (row.stt === 10 && row.type == "textarea") {
            bodyHTML += `<td class="label-col">
                <span class="label-text">${row.label}</span>
                <span class="label-english">${row.labelEn}</span>
            </td>`;
            dataArray.forEach((item, colIndex) => {
                bodyHTML += '<td colspan="4" class="value-col">';
                const val1 = item[row.field1] || '';
                const val2 = item[row.field2] || '';
                const val3 = item[row.unit1] || '';
                const val4 = item[row.unit2] || '';
                bodyHTML += `
                <div class="row-flex-nowrap mb-1">
                    <div class="input-main">
                        <textarea style="color: #FF3A00; text-align: center;" class="form-control" 
                            rows="1" readonly>${val1}</textarea>
                    </div>
                    <div class="unit-box d-flex align-items-center justify-content-center">${val3}</div>
        
                    <div class="input-main ms-2" style="border-left: 1px solid #ccc; padding-left: 10px">
                        <textarea style="color: #FF3A00; text-align: center;" class="form-control" 
                            rows="1" readonly>${val2}</textarea>
                    </div>
                    <div class="unit-box d-flex align-items-center justify-content-center">${val4}</div>
                </div>`;
                bodyHTML += '</td>';
            });
        }

        else if (row.stt === 10 && row.type === "textarea-with-radio") {
            bodyHTML += `<td class="label-col">
                <span class="label-text">${row.label}</span>
                <span class="label-english">${row.labelEn}</span>
            </td>`;

            dataArray.forEach((item, colIndex) => {
                bodyHTML += '<td colspan="4" class="value-col">';

                const val1 = item[row.field1] || '';
                const val2 = item[row.field2] || '';
                const unit1 = item[row.unit1] || '';
                const unit2 = item[row.unit2] || '';

                // Trạng thái Radio 1 (Roll)
                const status1 = item[row.statusField1];
                const rName1 = `statusRoll-${colIndex}`;

                // Trạng thái Radio 2 (Kho)
                const status2 = item[row.statusField2];
                const rName2 = `statusKho-${colIndex}`;

                bodyHTML += `
                <div class="row">
                    <div class="col-6" style="border-right: 1px solid #ccc;">
                        <div class="row-flex-nowrap mb-1">
                            <div class="input-main">
                                <textarea style="color: #FF3A00; text-align: center;" class="form-control" rows="1" readonly>${val1}</textarea>
                            </div>
                            <div class="unit-box d-flex align-items-center justify-content-center">${unit1}</div>
                        </div>
                        <div class="d-flex align-items-center justify-content-center flex-wrap">
                            <div class="form-check form-check-inline div-radio">
                                <input class="form-check-input no-click no-click" type="radio" name="${rName1}" ${status1 == 1 ? 'checked' : ''}>
                                <label class="form-check-label text-success small">Pass</label>
                            </div>
                            <div class="form-check form-check-inline div-radio">
                                <input class="form-check-input no-click no-click" type="radio" name="${rName1}" ${status1 == 0 ? 'checked' : ''}>
                                <label class="form-check-label text-danger small">Fail</label>
                            </div>
                            <div class="form-check form-check-inline div-radio">
                                <input class="form-check-input no-click no-click" type="radio" name="${rName1}" ${status1 == 2 ? 'checked' : ''}>
                                <label class="form-check-label text-secondary small">No</label>
                            </div>
                        </div>
                    </div>

                    <div class="col-6">
                        <div class="row-flex-nowrap mb-1">
                            <div class="input-main">
                                <textarea style="color: #FF3A00; text-align: center;" class="form-control" rows="1" readonly>${val2}</textarea>
                            </div>
                            <div class="unit-box d-flex align-items-center justify-content-center">${unit2}</div>
                        </div>
                        <div class="d-flex align-items-center justify-content-center flex-wrap ">
                            <div class="form-check form-check-inline div-radio">
                                <input class="form-check-input no-click no-click" type="radio" name="${rName2}" ${status2 == 1 ? 'checked' : ''}>
                                <label class="form-check-label text-success small">Pass</label>
                            </div>
                            <div class="form-check form-check-inline div-radio">
                                <input class="form-check-input no-click no-click" type="radio" name="${rName2}" ${ status2 == 0 ? 'checked' : ''}>
                                <label class="form-check-label text-danger small">Fail</label>
                            </div>
                            <div class="form-check form-check-inline div-radio">
                                <input class="form-check-input no-click no-click" type="radio" name="${rName2}" ${ status2 == 2 ? 'checked' : ''}>
                                <label class="form-check-label text-secondary small">No</label>
                            </div>
                        </div>
                    </div>
                </div>`;
                bodyHTML += '</td>';
            });
        }

        // Xử lý các dòng bình thường
        else {
            bodyHTML += `<td class="label-col">
                <span class="label-text">${row.label}</span>
                <span class="label-english">${row.labelEn}</span>
            </td>`;

            // Các cột kết quả cho từng loại vải
            dataArray.forEach((item, colIndex) => {
                bodyHTML += '<td colspan="4" class="value-col">';

                switch (row.type) {
                    case 'static':
                        const value = typeof row.field === 'function' ? row.field(item) : item[row.field];
                        bodyHTML += `<div style="min-height: 30px; color: red;">${value || ''}</div>`;
                        break;

                    case 'radio-text':
                        const radioId = `${row.radioName}-${colIndex}`;
                        const status = item[row.statusField];
                        const textValue = item[row.valueField] || '';
                        const img = item[row.dataImage]
                        // Lưu ảnh
                        let radioHtml = '';
                        if (row.stt == 9) {
                            radioHtml = `
                                <div class="form-check form-check-inline div-radio">
                                    <input class="form-check-input no-click" type="radio" name="${radioId}" id="${radioId}-pass" value="pass" ${status === 1 ? 'checked' : ''}>
                                    <label class="form-check-label text-success">Yes</label>
                                </div>
                                <div class="form-check form-check-inline div-radio">
                                    <input class="form-check-input no-click" type="radio" name="${radioId}" id="${radioId}-fail" value="fail" ${status === 0 ? 'checked' : ''}>
                                    <label class="form-check-label text-danger">No</label>
                                </div>`;
                        }
                        else {
                                    radioHtml = `
                                <div class="form-check form-check-inline div-radio">
                                    <input class="form-check-input no-click" type="radio" name="${radioId}" id="${radioId}-pass" value="pass" ${status === 1 ? 'checked' : ''}>
                                    <label class="form-check-label text-success">Pass</label>
                                </div>
                                <div class="form-check form-check-inline div-radio">
                                    <input class="form-check-input no-click" type="radio" name="${radioId}" id="${radioId}-fail" value="fail" ${status === 0 ? 'checked' : ''}>
                                    <label class="form-check-label text-danger">Fail</label>
                                </div>
                                <div class="form-check form-check-inline div-radio ">
                                    <input class="form-check-input no-click" type="radio" name="${radioId}" id="${radioId}-no" value="no" ${status === 2 ? 'checked' : ''}>
                                    <label class="form-check-label text-secondary">No</label>
                                </div>`;
                                            }

                                            bodyHTML += `
                            <div class="d-flex align-items-center row">
                                <div class="col-md-12 col-lg-6 col-xl-6 m-0 p-0">
                                    <div class="d-flex align-items-center justify-content-around">
                                        <div class="camera-image-container" onclick="handleImageClick(this)">
                                            ${img ? `<img src="/Images/SignKiemVai/${img}" alt="Image">` : ""}
                                        </div>
                                        <div class="d-flex align-items-start flex-column gap-1">
                                            ${radioHtml}
                                        </div>
                                    </div>
                                </div>
                                <div class="col-12 col-lg-6 col-xl-6">
                                    <textarea style="color:green" class="form-control flex-grow-1"
                                        data-field="${row.valueField}" data-col="${colIndex}" 
                                        rows="1" readonly>${textValue}</textarea>
                                </div>
                            </div>`;
                        break;

                    case 'radio-input-only':
                        const rId = `${row.radioName}-${colIndex}`;

                        // Lấy giá trị từ API dựa trên statusField (TrongLuong, DoCoRut, hoặc ChongTham)
                        const apiValue = item[row.statusField];

                        // Logic kiểm tra: 1: Pass, 2: Fail, (0 hoặc null/undefined): No
                        const isPass = apiValue == 1;
                        const isFail = apiValue == 2;
                        const isNo = (apiValue == 0 || apiValue == null || apiValue == undefined);

                        bodyHTML += `
                            <div class="d-flex flex-column gap-2">
                                <div class="d-flex align-items-center justify-content-center gap-3">
                                    <div class="form-check form-check-center m-0">
                                        <input class="form-check-input no-click" type="radio" name="${rId}" id="${rId}-pass" value="pass" ${isPass ? 'checked' : ''}>
                                        <label class="form-check-label text-success">Pass</label>
                                    </div>
                                    <div class="form-check form-check-center m-0">
                                        <input class="form-check-input no-click" type="radio" name="${rId}" id="${rId}-fail" value="fail" ${isFail ? 'checked' : ''}>
                                        <label class="form-check-label text-danger">Fail</label>
                                    </div>
                                   
                                    <div class="form-check form-check-center m-0 }">
                                        <input class="form-check-input no-click" type="radio" name="${rId}" id="${rId}-no" value="no" ${isNo ? 'checked' : ''}>
                                        <label class="form-check-label text-secondary">No</label>
                                    </div>
                                </div>
                            </div>`;
                        break;
                    case 'textarea':
                        const textVal = item[row.field] || '';
                        const unitVal = item[row.unit] || 'm';

                        if (row.hideUnit) {
                            bodyHTML += `
                            <div class="row no-gutters">
                                <div class="col-12">
                                    <textarea style="color:red; text-align: center;" 
                                        class="form-control"
                                        data-field="${row.field}" 
                                        data-col="${colIndex}" 
                                        rows="1" readonly>${textVal}</textarea>
                                </div>
                            </div>`;
                                        } else {
                                            // 👉 hiển thị bình thường (STT khác)
                            bodyHTML += `
                            <div class="row-flex-nowrap">
                                <div class="input-main">
                                    <textarea style="color:red; text-align: center;" 
                                        class="form-control"
                                        data-field="${row.field}" 
                                        data-col="${colIndex}" 
                                        rows="1" readonly>${textVal}</textarea>
                                </div>
                                <div class="unit-box d-flex align-items-center justify-content-center">${unitVal}</div>
                            </div>`;
                        }
                        break;

                    case 'checkbox':
                        const checked = item[row.field] == 1;
                        const checkId = `${row.field}-${colIndex}`;
                        bodyHTML += `
                            <div class="form-check">
                                <input class="form-check-input no-click mt-0" type="checkbox" 
                                    id="${checkId}" data-field="${row.field}" data-col="${colIndex}"
                                    ${checked ? 'checked' : ''} >
                                <label class="form-check-label ms-2"></label>
                            </div>`;
                        break;
                }

                bodyHTML += '</td>';
            });
        }

        bodyHTML += '</tr>';
    });

    // ===== THÊM CÁC DÒNG DEFECT ĐỘNG =====
    if (dtThongSoloi && dtThongSoloi.length > 0) {
        // Lấy danh sách defect types unique
        const uniqueDefects = [...new Set(dtThongSoloi.map(d => d.ErrorType))];

        uniqueDefects.forEach((defectType, defectIndex) => {
            const defectInfo = defectMapping[defectType] || { vi: defectType, en: defectType };

            bodyHTML += `<tr data-defect="${defectType}">`;
            bodyHTML += `<td  class="row-number">${14}</td>`;

            // Cột tên lỗi
            bodyHTML += `<td class="defect-name">
                <span class="defect-name-vi">${defectInfo.vi}</span>
                <span class="defect-name-en">${defectInfo.en}</span>
                ${defectInfo.note ? `<p class="mb-0" style="font-size: 11px;"><strong>${defectInfo.note}</strong></p>` : ''}
            </td>`;

            // Các cột điểm cho mỗi loại vải
            dataArray.forEach((item, colIndex) => {
                // Tìm defect data cho IDErrorType này
                const defectData = dtThongSoloi.find(d =>
                    d.IDErrorType === item.IDErrorType && d.ErrorType === defectType
                );

                if (defectData && defectData.ErrorPoint) {
                    // Parse ErrorPoint: "1@1@1@1" -> [1, 1, 1, 1]
                    const points = defectData.ErrorPoint.split('@');

                    // Render 4 cột điểm (1, 2, 3, 4)
                    for (let i = 0; i < 4; i++) {
                        const pointValue = points[i] || '0';
                        bodyHTML += `<td class="point-cell">
                            <span class="counter-value" data-point="${i + 1}">${pointValue == 0 ? "" : pointValue}</span>
                        </td>`;
                    }
                } else {
                    // Nếu không có data, hiển thị 0
                    for (let i = 0; i < 4; i++) {
                        bodyHTML += `<td class="point-cell">
                            <span class="counter-value" data-point="${i + 1}"></span>
                        </td>`;
                    }
                }
            });

            bodyHTML += '</tr>';
        });
    }

    bodyHTML += `<tr><td class="row-number">15</td>`;
    bodyHTML += `<td class="label-col"><span class="label-text">Kết quả /</span> <span class="label-english">Result</span></td>`;

    dataArray.forEach((item, colIndex) => {
        const diemLoi = item.ErrorPoint || 0;
        const status = item.StatusResult;

        let statusText = "";
        let statusClass = "";
        if (status == 1) { statusText = "ĐẠT"; statusClass = "text-success"; }
        else if (status == 0 || status == 2) { statusText = "KHÔNG ĐẠT"; statusClass = "text-danger"; }

        bodyHTML += `<td colspan="4" class="value-col text-center pr-4" style="vertical-align: middle;">
            <span style="font-weight:bold; margin-right: 15px;">Điểm: <span style="color:red">${diemLoi}</span></span>
            <span class="${statusClass}">${statusText}</span>
        </td>`;
    });
    bodyHTML += `</tr>`;

    bodyHTML += `<tr><td class="row-number">16</td>`;
    bodyHTML += `<td class="label-col"><span class="label-text">Kết luận/</span> <span class="label-english">Final Decision</span></td>`;

    dataArray.forEach((item, colIndex) => {
        // Logic tự động kiểm tra xem có lỗi nào không để check mặc định
        const fieldsToCheck = [
            item.ResultQCStatus
        ];
        const hasFail = fieldsToCheck.some(status => status === 2);
        const radioGroupName = `finalDecision-${colIndex}`;

        bodyHTML += `
        <td colspan="4" class="value-col" style="vertical-align: middle;">
            <div class="d-flex justify-content-center gap-2">
                <div class="form-check form-check-center">
                    <input class="form-check-input no-click no-click" type="radio" 
                        name="${radioGroupName}" id="${radioGroupName}-pass" 
                        value="1" ${!hasFail ? 'checked' : ''}>
                    <label class="form-check-label text-success">PASS</label>
                </div>
                <div class="form-check form-check-center">
                    <input class="form-check-input no-click no-click" type="radio" 
                        name="${radioGroupName}" id="${radioGroupName}-fail" 
                        value="0" ${hasFail ? 'checked' : ''}>
                    <label class="form-check-label text-danger">FAIL</label>
                </div>
            </div>
        </td>`;
    });
    bodyHTML += `</tr>`;

    bodyHTML += `<tr><td class="row-number">17</td>`;
    bodyHTML += `<td class="label-col"><span class="label-text">Kết luận Mer/</span> <span class="label-english">Mer Results</span></td>`;

    dataArray.forEach((item, colIndex) => {
        // Logic tự động kiểm tra xem có lỗi nào không để check mặc định
        const fieldsToCheck = [
            item.KetLuan_Mer
        ];
        const hasFail = fieldsToCheck.some(status => status === 2);
        const radioGroupName = `finalDecision-${colIndex}`;
        console.log(item.KetLuan_Mer)
        bodyHTML += `
        <td colspan="4" class="value-col" style="vertical-align: middle;">
            <div class="d-flex justify-content-center gap-2">
                <div class="form-check form-check-center">
                    <input class="form-check-input no-click no-click" type="radio" 
                        name="${radioGroupName}" id="${radioGroupName}-pass" 
                        value="1" ${item.KetLuan_Mer == "1" ? 'checked' : ''}>
                    <label class="form-check-label text-success">PASS</label>
                </div>
                <div class="form-check form-check-center">
                    <input class="form-check-input no-click no-click" type="radio" 
                        name="${radioGroupName}" id="${radioGroupName}-fail" 
                        value="0" ${item.KetLuan_Mer == "2" ? 'checked' : ''}>
                    <label class="form-check-label text-danger">FAIL</label>
                </div>
            </div>
        </td>`;
    });
    bodyHTML += `</tr>`;
    
    const hasFail = dataArray.some(status => status.ResultQCStatus === 2);
    if (hasFail) {
        $("#resultQC-pass").prop("checked", false);
        $("#resultQC-fail").prop("checked", true)
    } else {
        $("#resultQC-pass").prop("checked", true);
        $("#resultQC-fail").prop("checked", false)
    }

    bodyHTML += `<tr>
        <td colspan="2" class="label-col">
            <span class="label-text">Note (ghi chú Mer)</span>
        </td>`;
    dataArray.forEach((item, colIndex) => {
        const noteValue = item.GhiChu_Mer || '';
        bodyHTML += `<td colspan="4" class="value-col">
            <textarea  style="color:" class="form-control text-left"
                data-field="NoteMer" data-col="${colIndex}" 
                rows="1" readonly>${noteValue ?? ""}</textarea>
        </td>`;
    });
    bodyHTML += '</tbody>';

    bodyHTML += `<tr>
        <td colspan="2" class="label-col">
            <span class="label-text">Note (ghi chú): ghi rõ số lỗi kiểm và mức độ ánh màu so với mẫu duyệt /</span>
            <span class="label-english">State total defects, total points, and color levels compared to the approved sample</span>
        </td>`;

    // Thêm textarea cho mỗi loại vải
    dataArray.forEach((item, colIndex) => {
        const noteValue = item.Note || '';
        bodyHTML += `<td colspan="4" class="value-col">
            <textarea  style="color:" class="form-control text-left"
                data-field="Note" data-col="${colIndex}" 
                rows="2" readonly>${noteValue}</textarea>
        </td>`;
    });
    bodyHTML += '</tbody>';
    // Render vào table
    $('.inspection-table').html(headerHTML + bodyHTML);
    $('.inspection-table').find('textarea').attr('readonly', 'readonly');
    $('.inspection-table').find('input[type="radio"]').addClass('no-click');
    $('.inspection-table').find('input[type="checkbox"]').addClass('no-click');
    mergeSttCells()
}

function mergeSttCells() {
    const rows = $('.inspection-table tbody tr');
    let currentStt = null;
    let startIndex = 0;
    let count = 0;

    rows.each(function (index) {
        const sttCell = $(this).find('td.row-number');
        const sttValue = sttCell.text().trim();

        if (sttValue === currentStt) {
            // Nếu STT trùng với dòng trước, ẩn cell này
            sttCell.hide();
            count++;
        } else {
            // Nếu STT khác, merge các cell trước đó
            if (count > 0) {
                const firstCell = $(rows[startIndex]).find('td.row-number');
                firstCell.attr('rowspan', count + 1);
                firstCell.css({
                    'vertical-align': 'middle',
                    'text-align': 'center'
                });
            }

            // Reset cho nhóm mới
            currentStt = sttValue;
            startIndex = index;
            count = 0;
        }

        // Xử lý dòng cuối cùng
        if (index === rows.length - 1 && count > 0) {
            const firstCell = $(rows[startIndex]).find('td.row-number');
            firstCell.attr('rowspan', count + 1);
            firstCell.css({
                'vertical-align': 'middle',
                'text-align': 'center'
            });
        }
    });
}
$('.text-result.form-control').not('#modalTextarea').on('click', function (e) {
    e.preventDefault();
    currentTextarea = $(this);
    var label = currentTextarea.closest('tr').find('.label-text').text().trim();

    $('#modalTextarea').val(currentTextarea.val());
    $('#modalFieldLabel').text(label);
    // Mở modal
    $('#inputModal').modal('show');
    // Focus vào textarea trong modal
});
// Khi nhấn nút Xác nhận
$('#btnConfirm').on('click', function () {
    if (currentTextarea) {
        var newValue = $('#modalTextarea').val();
        currentTextarea.val(newValue);
        currentTextarea.text(newValue);
    }
    $('#inputModal').modal('hide');
});

// Xóa currentTextarea khi đóng modal
$('#inputModal').on('hidden.bs.modal', function () {
    currentTextarea = null;
    $('#modalTextarea').val('');
});

// Cho phép nhấn Enter để xuống dòng trong modal textarea
$('#modalTextarea').on('keydown', function (e) {
    if (e.keyCode === 14) {
        e.stopPropagation();
    }
});
$("#inputModal").on('shown.bs.modal', function () {
    $("#modalTextarea").removeAttr('readonly');  // Dùng # cho id, và removeAttr thay vì attr('readonly', '')
    $("#modalTextarea").focus();
});
var imageSignClass;

function CallModal(className) {
    imageSignClass = className; // Lưu tên class
    $("#signatureModal").modal("show")
}


function Scroll() {
    window.scrollTo(0, scrollPosition);
}

async function SaveSign(imageData) {
    const mauVTID = $("#MauVai option:selected").data("mauvt")
    const maVTID = $("#MauVai option:selected").data("mavtid")

    var arr = []
    const item = {
        ID: 0,
        SoLoID: $("#SoLo").val(),
        LoaiVai: $("#PhuLieuA").val(),
        MaVTID: maVTID,
        MauVTID: mauVTID,
        Sign: imageSignClass == "image1" ? imageData : "",
        SignManager: imageSignClass == "image2" ? imageData : "",
        SignReceive: imageSignClass == "image3" ? imageData : "",
        MaNPL: $("#MauVai").val()
    };
    arr.push(item)
    const request = new Request(`/api/KiemVai/PostSign?action=PostSign&type=@TypeTableSign`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arr),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data.toUpperCase() == "TRUE") {
        showToast('success', `Lưu chữ ký thành công!`, 1500);
    }
    else {
        showToast('error', `Lưu chữ ký thất bại!`, 2000);
        return
    }
}
function renderImage() {
    const mauVTID = $("#MauVai option:selected").data("mauvt")

    return new Promise((resolve, reject) => {
        var post = {
            action: 'GetSignKiemVaiV2',
            para1: $("#SoLo").val(),
            para2: $("#MauVai").val(),
            para3: mauVTID
        };
        $.ajax({
            type: "GET",
            url: '/api/KiemVai/Get',
            data: ko.toJS(post),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (d) {
                resolve(d.dt1); // Resolve dữ liệu khi thành công
            },
            error: function (err) {
                reject(err); // Reject khi có lỗi
            }
        });
    });
}
async function Export() {
    let dataImage = await renderImage()

    let arrThongSo = await [
        $("#KhachHang").val(),
        $("#nhacungcap ").val(),
        $("#SoLo option:selected").text(),
      
        dataImage[0]?.Sign,
        dataImage[0]?.SignManager,
        dataImage[0]?.SignReceive,
        $("#resultQC-pass").is(":checked"),
        $("#resultQC-fail").is(":checked"),
         $(".text-result").val()
    ]
    let dataToSend = JSON.stringify({
        ArrBody: dataKiem,
        ArrBodyErrorPoint: dataErrorPoint,
        ArrThongSo: arrThongSo
    });

    var url = `/api/KiemVai/EXBCKTVaiV2`;
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: dataToSend,
    }).then(response => {

        return response.blob();
    }).then(blob => {
        var a = document.createElement("a");
        var url = window.URL.createObjectURL(blob);
        a.href = url;
        a.download = 'BaoCaoKTCLVai.xlsx';
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
    })

}
async function SaveResult() {
    const batchjoin = selectedItems.map(item => item.batch)
    const lotjoin = selectedItems.map(item => item.lot)
    const mauVTID = $("#MauVai option:selected").data("mauvt")
    var arr = []
    batchjoin.forEach((x, index) => {
        const item = {
            ID: 0,
            SoLoID: $("#SoLo").val(),
            LoaiVai: $("#PhuLieuA").val(),
            MaVTID: $("#MauVai").val(),
            MauVTID: mauVTID,
            LOT: lotjoin[index],
            Batch: x,
            ResultQC: $(".text-result").val(),
            StatusResultQC: $("#resultQC-pass").is(":checked") ? 1 : $("#resultQC-fail").is(":checked") ? 0 : null,
            MaNPL: maNPL 
        };
        arr.push(item)
    })
    await Save(arr, "UpdateResult", "@TypeTableV2")
}
async function Save(arrSave, action, type) {

    const request = new Request(`/api/KiemVai/PostKiemVaiV2?action=${action}&type=${type}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSave),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data == "true") {
        showToast('success', `Cập nhật  thành công!`, 1500);
    }
    else {
        showToast('error', `Cập nhật thất bại!`, 2000);
        return
    }
}
function showToast(type, message, delay = 2000) {
    let toastId, messageId;

    switch (type) {
        case 'success':
            toastId = 'successToast';
            messageId = 'successMessage';
            break;
        case 'error':
            toastId = 'errorToast';
            messageId = 'errorMessage';
            break;
        case 'warning':
            toastId = 'warningToast';
            messageId = 'warningMessage';
            break;
        default:
            console.error('Unknown toast type:', type);
            return;
    }

    $('#' + messageId).html(message);

    const toastElement = $('#' + toastId)[0];
    const toast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: delay
    });

    toast.show();
}


var canvas, ctx;
let isDrawing = false;
let penColor = '#000000';
let penSize = 2;
let signatureHistory = [];
let currentStroke = [];

$(document).ready(function () {
    canvas = document.getElementById('signatureCanvas');
    ctx = canvas.getContext('2d');

    // Khởi tạo kích thước canvas
    initCanvas();

    // Cấu hình canvas
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';

    // Sự kiện chuột
    $(canvas).on('mousedown', startDrawing);
    $(canvas).on('mousemove', draw);
    $(canvas).on('mouseup', stopDrawing);
    $(canvas).on('mouseleave', stopDrawing);

    // Sự kiện chạm (mobile)
    canvas.addEventListener('touchstart', handleTouchStart, { passive: false });
    canvas.addEventListener('touchmove', handleTouchMove, { passive: false });
    canvas.addEventListener('touchend', stopDrawing, { passive: false });

    // Cập nhật màu từ color picker
    $('#penColor').on('change', function () {
        penColor = $(this).val();
    });

   
});
// Reset canvas khi mở modal
$('#signatureModal').on('shown.bs.modal', function () {
    initCanvas();
    clearSignature();
});

// Resize canvas khi thay đổi kích thước màn hình
$(window).on('resize', function () {
    if ($('#signatureModal').hasClass('show')) {
        initCanvas();
    }
});
function initCanvas() {
    const container = canvas.parentElement;
    const containerWidth = container.offsetWidth;

    // Xác định kích thước canvas dựa trên màn hình
    if (window.innerWidth < 768) {
        canvas.width = Math.min(containerWidth - 40, 500);
        canvas.height = 250;
    } else {
        canvas.width = Math.min(containerWidth - 40, 700);
        canvas.height = 300;
    }

    // Cấu hình lại context sau khi thay đổi kích thước
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
}

function getMousePos(e) {
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    return {
        x: (e.clientX - rect.left) * scaleX,
        y: (e.clientY - rect.top) * scaleY
    };
}

function getTouchPos(e) {
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    return {
        x: (e.touches[0].clientX - rect.left) * scaleX,
        y: (e.touches[0].clientY - rect.top) * scaleY
    };
}

function startDrawing(e) {
    isDrawing = true;
    const pos = getMousePos(e);
    currentStroke = [{ x: pos.x, y: pos.y, color: penColor, size: penSize }];

    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function draw(e) {
    if (!isDrawing) return;

    const pos = getMousePos(e);
    currentStroke.push({ x: pos.x, y: pos.y, color: penColor, size: penSize });

    ctx.strokeStyle = penColor;
    ctx.lineWidth = penSize;
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}

function stopDrawing() {
    if (isDrawing && currentStroke.length > 0) {
        signatureHistory.push([...currentStroke]);
        currentStroke = [];
    }
    isDrawing = false;
}

function handleTouchStart(e) {
    e.preventDefault();
    isDrawing = true;
    const pos = getTouchPos(e);
    currentStroke = [{ x: pos.x, y: pos.y, color: penColor, size: penSize }];

    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function handleTouchMove(e) {
    e.preventDefault();
    if (!isDrawing) return;

    const pos = getTouchPos(e);
    currentStroke.push({ x: pos.x, y: pos.y, color: penColor, size: penSize });

    ctx.strokeStyle = penColor;
    ctx.lineWidth = penSize;
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}

function clearSignature() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    signatureHistory = [];
    currentStroke = [];
}

function undoSignature() {
    if (signatureHistory.length === 0) return;

    signatureHistory.pop();
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Vẽ lại tất cả nét còn lại
    signatureHistory.forEach(stroke => {
        if (stroke.length === 0) return;

        ctx.beginPath();
        ctx.moveTo(stroke[0].x, stroke[0].y);

        stroke.forEach((point, index) => {
            if (index === 0) return;
            ctx.strokeStyle = point.color;
            ctx.lineWidth = point.size;
            ctx.lineTo(point.x, point.y);
            ctx.stroke();
        });
    });
}

function setPenColor(color) {
    penColor = color;
    $('#penColor').val(color);
}

function setPenSize(size) {
    penSize = size;
    $('.pen-size-btn').removeClass('active');
    $(`.pen-size-btn[data-size="${size}"]`).addClass('active');
}

function saveSignature() {
    if (signatureHistory.length === 0) {
        alert('Vui lòng ký tên trước khi lưu!');
        return;
    }

    const signatureImage = canvas.toDataURL('image/png');
    const imgElement = document.querySelector('.' + imageSignClass);
    if (imgElement) {
        imgElement.src = signatureImage; // Gán hình vào đúng img
    }
    SaveSign(signatureImage)
    $('#signatureModal').modal('hide');

}






let currentStream = null;
let currentContainer = null;
let currentFacingMode = 'environment'; // 'environment' (camera sau) hoặc 'user' (camera trước)
const imageData = {};
function handleImageClick(container) {
    currentContainer = container; // Lưu DOM element
    const $container = $(container); // jQuery object


    const img = $container.find("img");
    if (img.length > 0) {

        const src = img.attr("src");
        showPreview(src);
    } else {
        console.log("Chưa có ảnh, mở modal chọn nguồn");
        showOptionModal();
    }
}
function showPreview(src) {
    $("#previewImage").attr("src", src);
    $("#previewModal").modal('show'); // Dùng jQuery addClass
}

function closePreview() {
    $("#previewModal").removeClass('show');
}

//Kiểm tra số cây trong từng batch/lot
async function checkMinFabricQuantity(items) {
    const mauVTID = $("#MauVai option:selected").data("mauvt");
    const invalidItems = [];

    const checks = items.map(async (item) => {
        const url = `/api/KiemVai/Get?action=GetCay&para1=${$("#SoLo").val()}&para2=${$("#MauVai").val()}&para3=${mauVTID}&para4=${item.batch}&para5=${item.lot}`;
        try {
            const response = await fetch(url);
            if (response.ok) {
                const data = await response.json();
                if (!data.dt1 || data.dt1.length < 2) {
                    invalidItems.push(`${data.dt1[0].Display}${data.dt1[0].SoKienHienThi}`);
                }
            }
        } catch (error) {
            console.error("Lỗi kiểm tra số cây:", error);
        }
    });

    await Promise.all(checks);
    return invalidItems;
}

async function checkAllMinFabricByAPI(data, mauVTID) {
    if (!data || data.length === 0) return;

    const soloVal = $("#SoLo").val();
    const mauVaiVal = $("#MauVai").val();
    let invalidList = [];

    const checks = data.map(async (item) => {
        const url = `/api/KiemVai/Get?action=CheckingSoCayBatchLot&para1=${soloVal}&para2=${mauVaiVal}&para3=${mauVTID}&para4=${item.MaNPL}`;

        try {
            const response = await fetch(url);
            if (response.ok) {
                const resData = await response.json();

                if (Array.isArray(resData.dt1)) {
                    resData.dt1.forEach(info => {
                        if (info.SoCayCanKiem > 1 && info.SoCayDaKiem < 2) {
                            if (info.SoLoT == item.SoLoT) {
                                invalidList.push(item.Display);
                            }
                        }
                    });
                }
            }
        } catch (error) {
            console.error("Lỗi khi check item:", item.Display, error);
        }
    });

    await Promise.all(checks);

    const alertBox = $("#minFabricAlert");
    if (invalidList.length > 0) {
        const message = `<strong>Thông báo:</strong> Các Batch/LOT sau có ít hơn 2 cây: <strong>${invalidList.join(", ")}</strong>.`;
        alertBox.find("div").html(message);
        alertBox.attr("style", "display: flex !important; padding: 5px !important; margin-bottom: 5px;");
    } else {
        alertBox.attr("style", "display: none !important;");
    }
    updateTableHeight();
}

//function displayFabricAlert(invalidList) {
//    const alertBox = $("#minFabricAlert");
//    if (invalidList.length > 0) {
//        const message = `Các Batch/LOT sau có ít hơn 2 cây: <strong>${invalidList.join(", ")}</strong>. Vui lòng cần ít nhất 2 cây.`;
//        alertBox.find("div").html(message);
//        alertBox.attr("style", "display: flex !important; padding: 5px !important; margin-bottom: 5px;");
//    } else {
//        alertBox.attr("style", "display: none !important; padding: 5px !important");
//    }
//    updateTableHeight();
//}

function updateTableHeight() {
    const fabricAlert = $("#minFabricAlert");
    const percentAlert = $("#percentInput");
    const tableContainer = $(".inspection-table").parent();

    // Nếu một trong hai hoặc cả hai đang hiển thị
    if (fabricAlert.is(':visible') || percentAlert.is(':visible')) {
        let offset = 125;
        if (fabricAlert.is(':visible')) offset += 40;
        if (percentAlert.is(':visible')) offset += 60;

        tableContainer.css("max-height", `calc(100vh - ${offset}px)`);
    } else {
        tableContainer.css("max-height", "calc(100vh - 125px)");
    }
}

//Kiểm tra vải kiểm >= 10% vải nhập
function checkLowInspectionRatio(data) {
    const alertBox = $("#percentInput");
    if (!data || data.length === 0) {
        alertBox.attr("style", "display: none !important;");
        return;
    }

    let warningList = [];

    data.forEach(item => {
        const nhap = parseFloat(item.SoLuongVaiNhap) || 0;
        const kiem = parseFloat(item.SoLuongVaiKiem) || 0;

        if (nhap > 0 && kiem < (nhap * 0.1)) {
            const ratio = nhap > 0 ? ((kiem / nhap) * 100).toFixed(1) : 0;
            warningList.push(`${item.Display} (Kiểm: ${ratio}%)`);
        }
    });

    if (warningList.length > 0) {
        const message = `<strong>Cảnh báo:</strong> Các Batch/Lot có tỉ lệ kiểm ít hơn 10%: <br> ${warningList.join(", ")}`;
        alertBox.find("div").html(message);
        alertBox.attr("style", "display: flex !important; padding: 5px !important; margin-bottom: 5px;");
    } else {
        alertBox.attr("style", "display: none !important;");
    }
    updateTableHeight();
}
$(function () {
    if (window.CefSharp) {
        $("#home").css("display", "none");
    }
})