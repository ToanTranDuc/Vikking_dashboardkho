let currentStream = null;
let currentContainer = null;
let currentFacingMode = 'environment'; // 'environment' (sau) hoặc 'user' (trước)
let imageData = {};
let fieldDelete = null;

let arrPhuLieu = new Array();
let arrSoLo = new Array();
var arrPhuLucHasUnit = [{ MaPhuLuc: "PhuLucPL_5" }, { MaPhuLuc: "PhuLucPL_6" },
    { MaPhuLuc: "PhuLucPL_7" }, { MaPhuLuc: "PhuLucPL_14" }]
let arrDonViVatTu = [];

function ResetVal(selector) {
    currentStream = null;
    currentContainer = null;
    currentFacingMode = 'environment'; // 'environment' (sau) hoặc 'user' (trước)
    imageData = {};
    fieldDelete = null;
    if (selector == 'SoLo') {
        arrPhuLieu = new Array();
        arrSoLo = new Array();
        rowSelected_PL = null;
    } else if (selector == 'PL') {
        rowSelected_PL = null;
    }
  


}
function GetDonViTinh() {
    $.ajax({
        async: false,
        url: `/api/QtyKiemPL/Get?action=GetDonViVT`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            arrDonViVatTu = []
            if (data.length > 0) {
                arrDonViVatTu = [...data]
            }

        }
    });
}


function fetchDonViVT(selector, selectedValue) {
    let options = '';
    if (Array.isArray(arrDonViVatTu)) {
        arrDonViVatTu.forEach(item => {
            const selected = item.MaDVVT === selectedValue ? 'selected' : '';
            options += `
                <option value="${item.MaDVVT}" ${selected}>
                    ${item.TenDVVT}
                </option>`;
        });
    }

    return `
        <select id="select-dv-${selector}" 
                class="form-select custom-input" disabled>
            ${options}
        </select>
    `;
}
function formatDateView(isoDate) {
    if (!isoDate) return "";

    let timestamp = Date.parse(isoDate);
    if (isNaN(timestamp)) return isoDate;

    let date = new Date(timestamp);
    let day = String(date.getDate()).padStart(2, '0');
    let month = String(date.getMonth() + 1).padStart(2, '0');
    let year = date.getFullYear();

    return `${day}-${month}-${year}`;
}

function formatTime(isoTimeString) {
    if (!isoTimeString) return "";

    let date = new Date(isoTimeString);
    if (isNaN(date.getTime())) return ""; // Kiểm tra nếu không phải ngày hợp lệ

    // Lấy giờ, phút, giây, đảm bảo luôn có 2 chữ số
    let hours = String(date.getHours()).padStart(2, '0');
    let minutes = String(date.getMinutes()).padStart(2, '0');
    let seconds = String(date.getSeconds()).padStart(2, '0');

    return `${hours}:${minutes}:${seconds}`;
}

function formatDateSQL(dateStr) {
    if (!dateStr) return "";

    if (dateStr instanceof Date) {
        return dateStr.toISOString().split("T")[0];
    }

    if (typeof dateStr !== "string") {
        console.error("Invalid dateStr:", dateStr);
        return "";
    }

    let parts = dateStr.split("-");
    if (parts.length !== 3) return "";

    let day = parts[0].padStart(2, "0");
    let month = parts[1].padStart(2, "0");
    let year = parts[2];

    return `${year}-${month}-${day}`;
}

function formatCurrencyValue(value) {
    if (value === null || value === undefined || value === '') return 0;

    const absVal = Math.abs(value);
    const isInteger = Number.isInteger(absVal);

    if (value < 0) {
        return isInteger
            ? `${absVal}`
            : `${absVal.toFixed(2)}`;
    }

    if (isInteger) return absVal.toString();


    return absVal.toFixed(2);
}

function InitComponent() {
    $("#selectSoLO").select2();
 
    let today = moment().format('DD-MM-YYYY');

    $('#ngay-kiem').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        autoApply: true,
        autoUpdateInput: false,
        locale: {
            format: 'DD-MM-YYYY',
            separator: ' - ',
            applyLabel: 'Chọn',
            cancelLabel: 'Hủy',
            fromLabel: 'Từ',
            toLabel: 'Đến',
            customRangeLabel: 'Tùy chỉnh',
            daysOfWeek: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            monthNames: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
            firstDay: 1
        }
    });

    // Gán giá trị mặc định = hôm nay
    $('#ngay-kiem').val(today);

    $("input[type=radio]").prop("disabled", true);
    $("textarea").prop("readonly", true);

}
window.addEventListener('resize', InitComponent);
window.addEventListener('load', InitComponent);


function GetSoLo() {
    $.ajax({
        async: false,
        url: `/api/QtyKiemPL/Get?action=GetSoLoBC`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            ResetVal('SoLo');
            if (data.length > 0) {
                arrSoLo = [...data]
                fetchSoLo(data)
                fetchFormSection(data[0]);
                GetPhieuKiemPL(data[0].SoLoID);
                //GetXacNhanKiem(data[0].MaPhieuKiem, data[0].SoLoID)
            }
            else {
                reloadPhuLieuGrid();
                fetchFormSection(null)
            }

        }
    });
}

function GetSoLoByDateRange(fromDate, toDate) {
    $.ajax({
        async: false,
        url: `/api/QtyKiemPL/Get?action=GetSoLoBCTheoNgay&para1=${fromDate}&para2=${toDate}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            ResetVal('SoLo');
            if (data.length > 0) {
                arrSoLo = [...data];
                fetchSoLo(data);
                if (data.length > 0) {
                    fetchFormSection(data[0]);
                    GetPhieuKiemPL(data[0].SoLoID);
                    //GetXacNhanKiem(data[0].MaPhieuKiem, data[0].SoLoID)
                }
            }
            else {
                reloadPhuLieuGrid();
                fetchFormSection(null)
              
            }
        },
        error: function (error) {
            console.error('Error GetSoLoByDateRange:', error);
            alert('Lỗi khi tải dữ liệu theo ngày');
        }
    });
}

function fetchSoLo(data) {
    $('#selectSoLO').empty();
    $.each(data, function (index, item) {
        $('#selectSoLO').append(
            `<option value="${item.SoLoID}">${item.SoLo}</option>`
        );
    });
    if (data.length > 0) {
        $('#selectSoLO').val(data[0].SoLoID)
    }
}

$('#selectSoLO').on("change", function () {
    const SoLoID = $('#selectSoLO').val();
   
    if (SoLoID) {
        const objSoLoSelected = arrSoLo.find(x => x.SoLoID == SoLoID);
        //GetPhuLieu(SoLoID);
        fetchFormSection(objSoLoSelected)
        GetPhieuKiemPL(SoLoID);
        //GetXacNhanKiem(objSoLoSelected.MaPhieuKiem, objSoLoSelected.SoLoID)
    }
})

function fetchFormSection(SoLo_infor) {
    $("#txt-khach-hang").val(SoLo_infor ? SoLo_infor.TenKH : ""  || "");
    $("#txt-nha-cc").val(SoLo_infor ? SoLo_infor.TenNCC  : "" || "");
    $("#ngay-nhap-kho").val(SoLo_infor ? formatDateView(SoLo_infor.NgayNK) : "" || "");
}
function GetXacNhanKiem(MaKiem,SoLoID) {
    $.ajax({
        async: false,
        url: `/api/QtyKiemPL/Get?action=GetXN_PL&para1=${MaKiem}&para2=${SoLoID}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {
                fetXacNhanKiem(data)
            } else {
                $("#txtDG_QC").val("");
                $("#txtDG_Mer").val("");
                $("#txtDG_KQ").val("");

                $("#KQPass").prop("checked", false);
                $("#KQFail").prop("checked", false);
                canvasIds.forEach(id => {
                    const pad = signaturePads[id];
                    if (pad) {
                        pad.clear();
                    }
                })
            }
            

        }
    });
}
function fetXacNhanKiem(data) {
    //if (!data || data.length === 0) return;

    const item = data[0]; 
    if (item.Result === true) {
        $("#KQPass").prop("checked", true);
        $("#KQFail").prop("checked", false);
    } else if (item.Result === false) {
        $("#KQPass").prop("checked", false);
        $("#KQFail").prop("checked", true);
    } else {
       
        $("#KQPass").prop("checked", false);
        $("#KQFail").prop("checked", false);
    }

    $("#txtDG_QC").val(item.KL_QC_Pass || '');
    $("#txtDG_Mer").val(item.KQ_Fail || '');
    $("#txtDG_KQ").val(item.KQ_GiaiQuyet || '');

    // Điền lại chữ ký (nếu có)
    const signMap = {
        'signQC': item.Sign_Pass,
        'signMer': item.Sign_Fail,
        'signXN_KQ': item.TPCL_ComfirmSign,
        'signTPCLPass': item.KQ_GiaiQuyet_Sign,
        'signTPCLFail': item.ReceivedInfo_Sign
    };

    canvasIds.forEach(id => {
        const pad = signaturePads[id];
        if (pad) {
            if (signMap[id]) {
                pad.fromDataURL(signMap[id]); 
            } else {
                pad.clear();
            }
        }
    });


}
function reloadPhuLieuGrid() {
    fetchPhieuKiemPL([])
    var $grid = $("#grvPhuLieuKiem");
    if ($grid.data("dxDataGrid")) {
        var grid = $grid.dxDataGrid("instance");
        grid.getDataSource().reload();

    }
    $("#txtDG_QC").val("");
    $("#txtDG_Mer").val("");
    $("#txtDG_KQ").val("");

    $("#KQPass").prop("checked", false);
    $("#KQFail").prop("checked", false);

    canvasIds.forEach(id => {
        const pad = signaturePads[id];
        if (pad) {
            pad.clear();
        }
    })
    $("#selectSoLO").empty();

}
function GetPhieuKiemPL(SoLoID) {
    $.ajax({
        url: `/api/QtyKiemPL/Get?action=GetBC&para1=${SoLoID}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            $("#text-note").val();
            ResetVal('PL');
            if (data.length > 0) {
                console.log(data)
                fetchPhieuKiemPL(data)
            } else {
                reloadPhuLieuGrid();
            }

        }
    });
}


function ParsePhuLieuFromString(data) {
    let arrPhuLieu = JSON.parse(JSON.stringify(data)); 

    $.each(arrPhuLieu, function (index, item) {
   
        if (item.STT === 0 || item.MaPhuLuc === "NONE") {
            item.STT = "";
        }

   
        Object.keys(item).forEach(function (key) {
            if (key.includes('@MaNPL@')) {
                const contentValue = item[key];
                let arrContentKiem = [];

                
                if (typeof contentValue === 'string' && contentValue) {
                    arrContentKiem = contentValue.split('@');
                }

             
                item[key + '_PARSED'] = ParseByMaPhuLuc(
                    item.MaPhuLuc,
                    arrContentKiem,
                    item
                );
            }
        });
    });

    return arrPhuLieu;
}


function ParseByMaPhuLuc(maPhuLuc, arrContent, item) {

    const IsHasUnit = arrPhuLucHasUnit.find(
        x => x.MaPhuLuc === maPhuLuc
    ) !== undefined;

    const dvvt = arrContent[3] == "@" || !arrContent[3] ? "" : arrContent[3].trim();
    const ObjectFind = !arrDonViVatTu || arrDonViVatTu.length == 0 ? "" : arrDonViVatTu.find(x => x.MaDVVT == dvvt);
    const defaultObj = {
        weightStatus: "",
        weightNote: "",
        img: "",
        isCheck: false,
        displayType: "text", // text | check | image | somet | wash,
        IsHasUnit: IsHasUnit,
        tenDVVT: ObjectFind ? ObjectFind.TenDVVT : ""
    };


    const note = arrContent[0] == "@" || !arrContent[0] ? "" : arrContent[0].trim();
    const status = arrContent[1] == "@" || !arrContent[1] ? "" : arrContent[1].trim();
    const img = arrContent[2] == "@" || !arrContent[2] ? "" : arrContent[2].trim();

    const soMet = arrContent[4] == "@" || !arrContent[4] ? "" : arrContent[4].trim();
    const cuon = arrContent[5] == "@" || !arrContent[5] ? "" : arrContent[5].trim();
    const isWash = arrContent[6] == "@" || !arrContent[6] ? "" : arrContent[6].trim();

    switch (maPhuLuc) {
        case 'PhuLucPL_1': // Chỉ có ảnh + note
            return {
                ...defaultObj,
                weightNote: note,
                img: img,
                displayType: "image"
            };
        case 'PhuLucPL_4': // Số Lot
        case 'PhuLucPL_5': // Số lượng
        case 'PhuLucPL_6': // Số lượng kiểm
        case 'PhuLucPL_7': // Khổ vải
            return {
                ...defaultObj,
                weightNote: note,
                displayType: "text"
            };
        case 'PhuLucPL_2': // Item Code - Pass/Fail/No
        case 'PhuLucPL_3': // Màu VT - Pass/Fail/No
        case 'PhuLucPL_8':  // Pass/Fail/No + Image
        case 'PhuLucPL_9':
        case 'PhuLucPL_10':
        case 'PhuLucPL_11':
        case 'PhuLucPL_12':
        case 'PhuLucPL_13':
        case 'NONE':
        case 'PhuLucPL_15':
            return {
                ...defaultObj,
                weightStatus: status || "none",
                weightNote: note,
                img: img,
                isCheck: true,
                displayType: "check"
            };

        case 'PhuLucPL_14': // Số mét thực tế + Số cuộn
            return {
                ...defaultObj,
                weightNote: `Tổng mét thực tế: ${soMet} | Số cuộn: ${cuon}`,
                soMet: soMet,
                cuon: cuon,
                dvvt: dvvt,
                displayType: "somet"
            };

        case 'PhuLucPL_16': // Wash test
      
            const washStatus = status === "pass" ? 1 : status === "fail" ? 0 : 2;
            return {
                ...defaultObj,
                weightStatus: washStatus,
                weightNote: ``,
                img: img,
                isCheck: true,
                isWash: isWash,
                displayType: "wash"
            };

        default:
            return {
                ...defaultObj,
                weightNote: note,
                displayType: "text"
            };
    }
   
}


function RenderCellHTML(parsedData, maPhuLuc) {
    let html = '<div class="d-flex align-items-center h-100 px-2 py-2 gap-3">';  
    const arrHasCamera = ["PhuLucPL_1", "PhuLucPL_8", "PhuLucPL_9", "PhuLucPL_10",
        "PhuLucPL_11", "PhuLucPL_12", "PhuLucPL_13", "PhuLucPL_15", "PhuLucPL_2", "PhuLucPL_3"];
    const hasCamera = arrHasCamera.includes(maPhuLuc);

  
    if (hasCamera || parsedData.displayType === "image") {
        const imgHtml = parsedData.img
            ? `<img src="${parsedData.img}" class="preview-img rounded border shadow-sm" 
                    style="height:58px; width:80px; object-fit:cover; cursor:pointer;" 
                    onclick="handleImageView(this)">`
            : `<div class="preview-img bg-light border rounded d-flex align-items-center justify-content-center" 
                    style="height:58px; width:80px;">
                   <i class="fas fa-camera text-muted fs-5"></i>
               </div>`;

        html += `<div class="flex-shrink-0">${imgHtml}</div>`;
    }

  
    if (parsedData.isCheck && parsedData.displayType !== "wash") {
        const currentStatus = (parsedData.weightStatus || '').toLowerCase();
        const passClass = currentStatus === 'pass' ? 'text-success fw-bold' : 'text-muted opacity-50';
        const failClass = currentStatus === 'fail' ? 'text-danger fw-bold' : 'text-muted opacity-50';
        const noClass = (currentStatus === 'no' || currentStatus === 'none' || !currentStatus)
            ? 'text-warning fw-bold' : 'text-muted opacity-50';
        html += `
    <div class="d-flex align-items-center gap-3 px-2 flex-shrink-0">
        <div class="d-flex align-items-center gap-1">
            <i class="fas fa-check-circle ${passClass}" style="font-size:1.1rem;"></i>
            <span class="${passClass}" style="font-size:0.9rem;">Pass</span>
        </div>
        <div class="d-flex align-items-center gap-1">
            <i class="fas fa-times-circle ${failClass}" style="font-size:1.1rem;"></i>
            <span class="${failClass}" style="font-size:0.9rem;">Fail</span>
        </div>
        ${maPhuLuc == "NONE" ? "" : `
        <div class="d-flex align-items-center gap-1">
            <i class="fas fa-minus-circle ${noClass}" style="font-size:1.1rem;"></i>
            <span class="${noClass}" style="font-size:0.9rem;">No</span>
        </div>`}
    </div>
`;

       
    }

    // 3. HIỂN THỊ WASH TEST (Yes/No + Pass/Fail)
    if (parsedData.displayType === "wash") {
        const washStatus = parseInt(parsedData.isWash);
        const isWashYes = washStatus === 1;
        const isWashNo = washStatus === 2;

        const currentStatus = parsedData.weightStatus;
        const passClass = currentStatus === 1 ? 'text-success fw-bold' : 'text-muted opacity-50';
        const failClass = currentStatus === 0 ? 'text-danger fw-bold' : 'text-muted opacity-50';

        html += `
            <div class="d-flex align-items-center gap-3 px-2 flex-shrink-0">
                <div class="d-flex align-items-center gap-1">
                    <i class="fas fa-${isWashYes ? 'check' : 'circle'}-circle text-${isWashYes ? 'success fw-bold' : 'muted opacity-50'}" 
                       style="font-size:1.1rem;"></i>
                    <span class="text-${isWashYes ? 'success fw-bold' : 'muted opacity-50'}" style="font-size:0.9rem;">Yes</span>
                </div>
                <div class="d-flex align-items-center gap-1">
                    <i class="fas fa-${isWashNo ? 'times' : 'circle'}-circle text-${isWashNo ? 'danger fw-bold' : 'muted opacity-50'}" 
                       style="font-size:1.1rem;"></i>
                    <span class="text-${isWashNo ? 'danger fw-bold' : 'muted opacity-50'}" style="font-size:0.9rem;">No</span>
                </div>
                ${!isWashNo ? `
                <div class="d-flex align-items-center gap-1">
                    <i class="fas fa-check-circle ${passClass}" style="font-size:1.1rem;"></i>
                    <span class="${passClass}" style="font-size:0.9rem;">Pass</span>
                </div>
                <div class="d-flex align-items-center gap-1">
                    <i class="fas fa-times-circle ${failClass}" style="font-size:1.1rem;"></i>
                    <span class="${failClass}" style="font-size:0.9rem;">Fail</span>
                </div>` : ''}
            </div>`;
    }

    // 4. HIỂN THỊ NOTE/TEXT
    if (parsedData.displayType !== "wash") {
        const noteValue = parsedData.weightNote || "";
        const noteHtml = noteValue
            ? `<span class="px-2" style="min-height:38px; font-size:0.9rem; color:#212529; white-space:pre-wrap; line-height:1.6;">${noteValue}</span>`
            : `<span class="text-muted text-center py-2" style="font-size:0.9rem;">—</span>`;

        html += `<span class="px-2 txt-pl" >${noteHtml}</span>`;
    }
   
    if (parsedData.IsHasUnit == true) {
        html += `<span class="txt-pl txt-dvt" >${parsedData.tenDVVT}</span>`;
    }

    html += '</div>';

    return html;
}

function fetchPhieuKiemPL(data) {

    const parsedData = ParsePhuLieuFromString(data);

    const fixedColumns = [
        {
            dataField: "STT",
            caption: "STT",
            minWidth: 60,
            width: 60,
            alignment: "center"
        },
        {
            dataField: "PhuLuc",
            caption: "Nội Dung Kiểm",
         
            cssClass: "col-phu-luc"
        }
    ];


    const dynamicColumns = [];
    if (parsedData.length > 0) {
        const sampleRow = parsedData[0];
        Object.keys(sampleRow).forEach(key => {
            if (key.includes('@MaNPL@') && !key.includes('_PARSED')) {
                // Tách tên cột từ key (VD: ZM16BL1@MaNPL@MACLVT_48@MAVT_942...)
                const keyParts = key.split('@MaNPL@');
                const columnCaption = keyParts[0] || "Kết quả";

                dynamicColumns.push({
                    dataField: key,
                    caption: `Lần ${keyParts[2]}`,
                   
                    cssClass: "col-phu-luc",
                    encodeHtml: false,
                    cellTemplate: function (container, options) {
                        const parsedKey = options.column.dataField + '_PARSED';
                        const parsedValue = options.data[parsedKey];
                        const maPhuLuc = options.data.MaPhuLuc;

                        if (parsedValue) {
                            const html = RenderCellHTML(parsedValue, maPhuLuc);
                            $(container).html(html);
                        } else {
                            $(container).html('<div class="text-muted text-center">—</div>');
                        }
                    }
                });
            }
        });
    }

    const allColumns = [...fixedColumns, ...dynamicColumns];
    $("#grvPhuLieuKiem").dxDataGrid({
        dataSource: parsedData,
        keyExpr: "MaPhuLuc",
        noDataText: "Chưa có dữ liệU",
        columns: allColumns,
        allowColumnResizing: true,
        columnAutoWidth: true,
        showBorders: true,
        showRowLines: true,
        showColumnLines: true,
        editing: {
            allowUpdating: false,
            allowDeleting: false,
            allowAdding: false
        },
        paging: { enabled: false },
        filterRow: { visible: false },
        searchPanel: { visible: false },
        scrolling: {
            mode: "standard",
            showScrollbar: "always",
            useNative: true
        },
        sorting: {
            mode: "none"
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.dataField === "STT") {
                var currentSTT = e.value;
                var rowIndex = e.rowIndex;

                if (rowIndex === 0 || e.component.cellValue(rowIndex - 1, "STT") !== currentSTT) {

                    var rowSpan = 1;
                    var nextIndex = rowIndex + 1;

                    while (nextIndex < e.component.totalCount() &&
                        e.component.cellValue(nextIndex, "STT") === currentSTT) {
                        rowSpan++;
                        nextIndex++;
                    }
                    if (rowSpan > 1) {
                        e.cellElement.attr("rowspan", rowSpan);
                        e.cellElement.css({
                            "vertical-align": "middle",
                            "text-align": "center"
                        });
                    }
                } else {

                    e.cellElement.hide();
                }
            }
        },
        onContentReady: function (e) {
            
            const noneRow = data.find(x => x.MaPhuLuc === "NONE");
            if (noneRow) {
                Object.keys(noneRow).forEach(key => {
                    if (key.includes('@MaNPL@')) {
                        const note = noneRow[key].split('@')[0];
                        if (note && note !== "@") {
                            $("#text-note-view").html(note);
                        }
                    }
                });
            }
        }
    });
}


function handleImageView(imgElement) {
    const imgSrc = $(imgElement).attr('src');
    if (imgSrc) {
        // Hiển thị modal Bootstrap để xem ảnh phóng to
        const modal = `
            <div class="modal fade" id="imageViewModal" tabindex="-1">
                <div class="modal-dialog modal-lg modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header bg-primary text-white">
                            <h5 class="modal-title">
                                <i class="fas fa-image me-2"></i>Xem ảnh
                            </h5>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body text-center bg-light p-4">
                            <img src="${imgSrc}" class="img-fluid rounded shadow" style="max-height:70vh; max-width:100%;">
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                <i class="fas fa-times me-1"></i>Đóng
                            </button>
                        </div>
                    </div>
                </div>
            </div>`;

      
        $('#imageViewModal').remove();

        
        $('body').append(modal);
        const modalInstance = new bootstrap.Modal(document.getElementById('imageViewModal'));
        modalInstance.show();

        $('#imageViewModal').on('hidden.bs.modal', function () {
            $(this).remove();
        });
    }
}

function savePhuLieuKiem() {
    const objSoLo = arrSoLo.find(x => x.SoLoID == $("#selectSoLO").val());
    const getFileName = `SL-${objSoLo.SoLoID}-Sign`;
    let dataUpload = [];
    let arrUpDatePhieu = [];
    var objPhieuPLSave = {};
    const today = formatDateSQL(new Date());

    for (const item of canvasIds) {
        const canvas = document.getElementById(item);  // hoặc $(`#${item}`)[0]
        if (canvas) {
            dataUpload.push({
                img: canvas.toDataURL('image/png'),  // ← đúng
                name: item
            });
        }
    }
    let selected = $('input[name="result-kiem"]:checked').val();
    let result = null;
    if (selected === 'pass') {
        result = true;
    } else if (selected === 'fail') {
        result = false;
    }
    
    $.ajax({
        url: `/api/QtyKiemPL/UploadImg?getFileName=${encodeURIComponent(getFileName)}`,
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataUpload),
        success: function (imagePaths) {        
            console.log("imagePaths:", imagePaths);
            const findImage = (keyword) => {
                const found = imagePaths.find(p => {
                    if (typeof p === 'string') return p.includes(keyword);
                    if (typeof p === 'object' && p.name && p.url) return p.name.includes(keyword);
                    return false;
                });
                return typeof found === 'string' ? found : found?.url || null;
            };
            objPhieuPLSave["MaPhieuKiem"] = objSoLo.MaPhieuKiem;
            objPhieuPLSave["SoLoID"] = objSoLo.SoLoID;
            objPhieuPLSave["NgayKiem"] = today;
            objPhieuPLSave["KL_QC_Pass"] = $("#txtDG_QC").val();
            objPhieuPLSave["NgayKiem_Pass"] = today;
            objPhieuPLSave["Sign_Pass"] = findImage('signQC');
            objPhieuPLSave["KQ_Fail"] = $("#txtDG_Mer").val();
            objPhieuPLSave["NgayKiem_Fail"] = today;
            objPhieuPLSave["Sign_Fail"] = findImage('signMer');
            objPhieuPLSave["TPCL_ComfirmDate"] = today;
            objPhieuPLSave["TPCL_ComfirmSign"] = findImage('signXN_KQ');
            objPhieuPLSave["KQ_GiaiQuyet"] = $("#txtDG_KQ").val();
            objPhieuPLSave["KQ_GiaiQuyet_Sign"] = findImage('signTPCLPass');
            objPhieuPLSave["ReceivedInfo_Date"] = today;
            objPhieuPLSave["ReceivedInfo_Sign"] = findImage('signTPCLFail');
         
            objPhieuPLSave["Result"] = result;
            arrUpDatePhieu.push(objPhieuPLSave);

            SavePhieu(arrUpDatePhieu)

        },
        error: function (xhr, status, err) {
            DevExpress.ui.notify('Không thể lưu chữ ký. Vui lòng thử lại!', 'warning', 2000);
        }
    });

}
function SavePhieu(data) {
    fetch('/api/QtyKiemPL/PostXN_DanhGia', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
        .then(res => res.ok ? res.json() : Promise.reject('Lỗi server'))
        .then(() => {
            DevExpress.ui.notify('Lưu thành công.', 'success', 1000);

        })
        .catch(error => {
            DevExpress.ui.dialog.alert('Lưu thất bại: ' + error, 'Lỗi');
            $("#btn-close-signature").click()
        });
}

$('#ngay-kiem').on('apply.daterangepicker', function (ev, picker) {
    $(this).val(picker.startDate.format('DD/MM/YYYY'));
});

$('#ngay-kiem').on('cancel.daterangepicker', function (ev, picker) {
    $(this).val(today); // hoặc '' nếu muốn xóa
});

/*Camera*/
// Click vào bất kỳ ảnh hoặc icon camera nào → mở preview
$(document).on('click', '.preview-img', function () {
    const rowKey = $(this).data('row');        // MaPhuLuc
    const colField = $(this).data('col');      // 5011@MaNPL@MACLVT_118@...

    if (!rowKey || !colField) return;

    const grid = $("#grvPhuLieuKiem").dxDataGrid("instance");
    const rowData = grid.getDataSource().items().find(x => x.MaPhuLuc === rowKey);
    if (!rowData || !rowData[colField]) return;

    const imgUrl = rowData[colField].img || "";

    if (imgUrl) {
        const previewImg = document.getElementById('previewImage');
        previewImg.src = imgUrl;
        new bootstrap.Modal(document.getElementById('previewModal')).show();
    }
});

$("#btn-refresh").on("click", function () {
    const SoLoID = $('#selectSoLO').val();
    const objSoLoSelected = arrSoLo.find(x => x.SoLoID == SoLoID);
    //GetPhuLieu(SoLoID);
    fetchFormSection(objSoLoSelected)
    GetPhieuKiemPL(SoLoID);
    //GetXacNhanKiem(objSoLoSelected.MaPhieuKiem, objSoLoSelected.SoLoID)
})


$("#btn-kiem-pl").on('click', function () {
    window.location.assign("/QtyKiemPL/PhieuKiem");
})

$("#btn-Excel").on("click", function () {
    const objSoLoSelected = arrSoLo.find(x => x.SoLoID == $("#selectSoLO").val());
    var url = `/api/QtyKiemPL/ExportBC`;
    const fileName = (() => {
        const now = new Date();
        const pad = n => n.toString().padStart(2, '0');
        return `BM04/QT13/CL01-${pad(now.getDate())}${pad(now.getMonth() + 1)}${now.getFullYear()}${pad(now.getHours())}${pad(now.getMinutes())}${pad(now.getSeconds())}.xlsx`;
    })();

    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(objSoLoSelected),
    })
        .then(response => {
            if (!response.ok) {
                alert("Lỗi Không thể xuất excel . Vui lòng thử lại!")
                return;
            }

            return response.blob();
        })
        .then(blob => {
            var a = document.createElement("a");
            var url = window.URL.createObjectURL(blob);
            a.href = url;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            console.error('Error fetching data from the server.', error);
        });
})


function getWeekNumber(date) {
    const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
    const dayNum = d.getUTCDay() || 7;
    d.setUTCDate(d.getUTCDate() + 4 - dayNum);
    const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
    return Math.ceil((((d - yearStart) / 86400000) + 1) / 7);
}

function getStartOfWeek(year, week) {
    const jan4 = new Date(year, 0, 4);
    const dayOfWeek = jan4.getDay() || 7;
    const startOfWeek1 = new Date(jan4);
    startOfWeek1.setDate(jan4.getDate() - dayOfWeek + 1);

    const startDate = new Date(startOfWeek1);
    startDate.setDate(startOfWeek1.getDate() + (week - 1) * 7);

    return startDate;
}


function getEndOfWeek(startDate) {
    const endDate = new Date(startDate);
    endDate.setDate(startDate.getDate() + 6);
    return endDate;
}

function formatDate(date) {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
}


function populateYears() {
    const yearSelect = document.getElementById('yearSelect');
    const currentYear = new Date().getFullYear();

    for (let year = currentYear - 2; year <= currentYear + 2; year++) {
        const option = document.createElement('option');
        option.value = year;
        option.textContent = `${year}`;
        if (year === currentYear) {
            option.selected = true;
        }
        yearSelect.appendChild(option);
    }
}

function updateWeeks() {
    const year = parseInt(document.getElementById('yearSelect').value);
    const weekSelect = document.getElementById('weekSelect');

    weekSelect.innerHTML = '';

    const lastDayOfYear = new Date(year, 11, 31);
    const totalWeeks = Math.max(52, getWeekNumber(lastDayOfYear));

    const today = new Date();
    const currentWeek = getWeekNumber(today);
    const currentYear = today.getFullYear();

    for (let week = 1; week <= totalWeeks; week++) {
        const startDate = getStartOfWeek(year, week);
        const endDate = getEndOfWeek(startDate);

        if (startDate.getFullYear() > year && week > 1) {
            break;
        }

        const option = document.createElement('option');
        option.value = week;
        option.setAttribute('data-year', year);
        option.textContent = `Tuần ${week} (${formatDate(startDate)} - ${formatDate(endDate)})`;

        if (year === currentYear && week === currentWeek) {
            option.selected = true;
        }

        weekSelect.appendChild(option);
    }
}

document.getElementById('filterTypeSelect').addEventListener('change', function () {
    const filterType = this.value;

    document.getElementById('piFilterContent').style.display = 'block';
    document.getElementById('dateFilterContent').style.display = 'none';
    document.getElementById('weekFilterContent').style.display = 'none';

    if (filterType === 'pi') {
        document.getElementById('piFilterContent').style.display = 'block';
    } else if (filterType === 'date') {
        document.getElementById('dateFilterContent').style.display = 'block';
    } else if (filterType === 'week') {
        document.getElementById('weekFilterContent').style.display = 'block';
    }

    applyFilter();
});

function applyFilter() {
    const filterType = document.getElementById('filterTypeSelect').value;
    if (filterType === 'pi') {
       
        GetSoLo();
    } else if (filterType === 'date') {

        onDateRangeChange();
    } else if (filterType === 'week') {
      
        onWeekChange();
    }
}

document.addEventListener('DOMContentLoaded', function () {
    populateYears();
    updateWeeks();

    // Set ngày hiện tại cho date inputs
    const today = new Date().toISOString().split('T')[0];
    document.getElementById('fromDate').value = today;
    document.getElementById('toDate').value = today;

   
    
});

function onDateRangeChange() {
    const fromDate = $('#fromDate').val();
    const toDate = $('#toDate').val();

    if (fromDate && toDate) {
        if (new Date(fromDate) > new Date(toDate)) {
            alert('Từ ngày phải nhỏ hơn hoặc bằng đến ngày');
            return;
        }
        GetSoLoByDateRange(fromDate, toDate);
    }
}

function onWeekChange() {
    const year = $('#yearSelect').val();
    const week = $('#weekSelect').val();

    if (year && week) {
        const startDate = getStartOfWeek(parseInt(year), parseInt(week));
        const endDate = getEndOfWeek(startDate);

        const fromDate = startDate.toISOString().split('T')[0];
        const toDate = endDate.toISOString().split('T')[0];

        console.log('Week filter:', fromDate, 'to', toDate);
        GetSoLoByDateRange(fromDate, toDate);
    }
}


let lastChecked = null;

$("input[name='result-kiem']").on("click", function () {
    if (lastChecked === this) {
       
        $(this).prop("checked", false);
        lastChecked = null;
    } else {

        lastChecked = this;
    }
});

$(document).ready(async function () {
    InitComponent();
    GetDonViTinh();
    $("#filterTypeSelect").val("date");
    applyFilter();
});