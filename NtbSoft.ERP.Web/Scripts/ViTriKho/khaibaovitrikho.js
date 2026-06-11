

// Dữ liệu lưu trữ với cấu trúc phân cấp
var trEditTang;
var trEditO;
var trEditOCreate;
var keLocal = ""
var dayLocal = ""
var checkWarehouse = 1
let warehouseData = {
    days: [],
    kes: [],
    tangs: [],
    os: []
};
function setActive(button) {
    var $btn = $(button);

    $('.btn-warehouse').removeClass('active');

    $btn.addClass('active');

    var warehouseName = $btn.text().trim();
    $('#warehouse-name').text(warehouseName);
    $('#selected-warehouse').show();

    var warehouse = $btn.data("warehouse");

    if (warehouse == "nguyen-lieu") checkWarehouse = 1;
    else if (warehouse == "phu-lieu") checkWarehouse = 2;
    else if (warehouse == "thanh-pham") checkWarehouse = 3;


    GetWarehouseData()
}
var userNameSave = localStorage.getItem("username1")
let currentSelectedDay = null;
let currentSelectedKe = null;
let currentSelectedTang = null;
let days;
let kes;
let tangs;
var dataUser = !window.CefSharp ? userNameSave : userName
var keClick;
var tangClick;
var dayClick;
$(document).ready(function () {
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })
    if (window.CefSharp) {
        $("#home").css("display", "none");
    }
    $("#selectToCopy").select2()
    $("#selectToCopyKe").select2()
    $("#selectToCopyTang").select2()
    GetWarehouseData()
    // Hàm xóa item (global function)
    window.deleteItem = function (type, id) {
        if (!confirm('Bạn có chắc chắn muốn xóa?')) return;

        switch (type) {
            case 'day':
                // Xóa tất cả items con
                const dayKes = warehouseData.kes.filter(ke => ke.dayId === id);
                dayKes.forEach(ke => {
                    const keTangs = warehouseData.tangs.filter(tang => tang.keId === ke.id);
                    keTangs.forEach(tang => {
                        warehouseData.os = warehouseData.os.filter(o => o.tangId !== tang.id);
                    });
                    warehouseData.tangs = warehouseData.tangs.filter(tang => tang.keId !== ke.id);
                });
                warehouseData.kes = warehouseData.kes.filter(ke => ke.dayId !== id);
                warehouseData.days = warehouseData.days.filter(day => day.id !== id);

                // Reset selected values if deleted item was selected
                if (currentSelectedDay === id) {
                    currentSelectedDay = null;
                    currentSelectedKe = null;
                    currentSelectedTang = null;
                }
                break;
            case 'ke':
                const keTangs = warehouseData.tangs.filter(tang => tang.keId === id);
                keTangs.forEach(tang => {
                    warehouseData.os = warehouseData.os.filter(o => o.tangId !== tang.id);
                });
                warehouseData.tangs = warehouseData.tangs.filter(tang => tang.keId !== id);
                warehouseData.kes = warehouseData.kes.filter(ke => ke.id !== id);

                // Reset selected values if deleted item was selected
                if (currentSelectedKe === id) {
                    currentSelectedKe = null;
                    currentSelectedTang = null;
                }
                break;
            case 'tang':
                warehouseData.os = warehouseData.os.filter(o => o.tangId !== id);
                warehouseData.tangs = warehouseData.tangs.filter(tang => tang.id !== id);

                // Reset selected values if deleted item was selected
                if (currentSelectedTang === id) {
                    currentSelectedTang = null;
                }
                break;
            case 'o':
                warehouseData.os = warehouseData.os.filter(o => o.id !== id);
                break;
        }

        updateDropdowns();
        toggleSectionState();
        showToast('success', 'Đã xóa thành công!');
    };

    // Hàm sửa item (placeholder)
    window.editItem = function (type, id) {
        showToast('success', 'Tính năng chỉnh sửa đang phát triển!');
    };

    // Event handlers cho dropdown changes - ĐÃ CẢI THIỆN
    $('#selectedDay').change(function () {
        currentSelectedDay = $(this).val();
        // Reset các cấp con khi thay đổi dãy
        currentSelectedKe = null;
        currentSelectedTang = null;
        updateDropdowns();

        let $tr = $(`#tbodyDay tr[data-id="${$('#selectedDay').val()}"]`);
        $tr.trigger("click")
    });

    $('#selectedKe').change(function () {
        currentSelectedKe = $(this).val();
        // Reset tầng khi thay đổi kệ
        currentSelectedTang = null;
        updateDropdowns();
        let $tr = $(`#tbodyKe tr[data-id="${$('#selectedKe').val()}"]`);
        $tr.trigger("click")
    });

    $('#selectedTang').change(function () {
        currentSelectedTang = $(this).val();
        updateInheritedDimensions();

        let $tr = $(`#tbodyTang tr[data-id="${$('#selectedTang').val()}"]`);
        $tr.trigger("click")
    });

    // Tạo dãy - ĐÃ CẢI THIỆN
    $('#createDay').click(function () {
        SaveDay()
    });
    // Tạo kệ - ĐÃ CẢI THIỆN
    $('#createKe').click(function () {
        SaveKe()
    });

    // Tạo tầng - ĐÃ CẢI THIỆN
    $('#createTang').click(function () {
        SaveTang()
    });

    // Tạo ô
    $('#createO').click(function () {
        SaveO()
    });

    updateDropdowns();
    toggleSectionState();
});
// Hàm tạo ID duy nhất
function generateId() {
    return 'id_' + Math.random().toString(36).substr(2, 9);
}

// Hàm hiển thị toast
function showToast(type, message) {
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

    $('#' + messageId).text(message);
    const toast = new bootstrap.Toast(document.getElementById(toastId));
    toast.show();
}



// Hàm cập nhật dropdown options - ĐÃ SỬA
function updateDropdowns() {
    // Lưu giá trị hiện tại trước khi cập nhật
    const previousDayValue = $('#selectedDay').val();
    const previousKeValue = $('#selectedKe').val();
    const previousTangValue = $('#selectedTang').val();

    // Update dãy dropdown
    $('#selectedDay').html('<option value="">Chọn dãy</option>');
    warehouseData.days.forEach(day => {
        $('#selectedDay').append(`<option value="${day.id}">${day.name}</option>`);
    });

    // Khôi phục giá trị đã chọn nếu vẫn tồn tại
    if (previousDayValue && warehouseData.days.find(d => d.id === previousDayValue)) {
        $('#selectedDay').val(previousDayValue);
        currentSelectedDay = previousDayValue;
    } else if (currentSelectedDay && warehouseData.days.find(d => d.id === currentSelectedDay)) {
        $('#selectedDay').val(currentSelectedDay);
    }

    // Update kệ dropdown based on selected day
    $('#selectedKe').html('<option value="">Chọn kệ</option>');
    if (currentSelectedDay) {
        const dayKes = warehouseData.kes.filter(ke => ke.dayId === currentSelectedDay);
        dayKes.forEach(ke => {
            $('#selectedKe').append(`<option value="${ke.id}">${ke.name}</option>`);
        });

        // Khôi phục giá trị kệ nếu vẫn tồn tại
        if (previousKeValue && dayKes.find(k => k.id === previousKeValue)) {
            $('#selectedKe').val(previousKeValue);
            currentSelectedKe = previousKeValue;
        } else if (currentSelectedKe && dayKes.find(k => k.id === currentSelectedKe)) {
            $('#selectedKe').val(currentSelectedKe);
        }
    }

    // Update tầng dropdown based on selected kệ
    $('#selectedTang').html('<option value="">Chọn tầng</option>');
    if (currentSelectedKe) {
        const keTangs = warehouseData.tangs.filter(tang => tang.keId === currentSelectedKe);
        keTangs.forEach(tang => {
            $('#selectedTang').append(`<option value="${tang.id}">${tang.name}</option>`);
        });

        // Khôi phục giá trị tầng nếu vẫn tồn tại
        if (previousTangValue && keTangs.find(t => t.id === previousTangValue)) {
            $('#selectedTang').val(previousTangValue);
            currentSelectedTang = previousTangValue;
        } else if (currentSelectedTang && keTangs.find(t => t.id === currentSelectedTang)) {
            $('#selectedTang').val(currentSelectedTang);
        }
    }

    // Cập nhật thông tin kích thước kế thừa
    updateInheritedDimensions();
}

// Hàm cập nhật kích thước kế thừa
function updateInheritedDimensions() {
    // Reset tất cả trường kế thừa
    $('#tangDai, #tangRong').val('');
    $('#oRong, #oCao').val('');

    // Cập nhật kích thước tầng từ kệ được chọn
    if (currentSelectedKe) {
        const ke = warehouseData.kes.find(k => k.id === currentSelectedKe);
        if (ke) {
            $('#tangDai').val(ke.dai);
            $('#tangRong').val(ke.rong);
        }
    }

    // Cập nhật kích thước ô từ tầng được chọn
    if (currentSelectedTang) {
        const tang = warehouseData.tangs.find(t => t.id === currentSelectedTang);
        var daySelect = $('#selectedDay option:selected').text().replace("Dãy", "")
        var keSelect = $('#selectedKe option:selected').text().replace("Kệ", "")
        var tangSelect = $('#selectedTang option:selected').text().replace("Tầng", "")
        var textO = `${daySelect.trim()}.${keSelect.trim()}.${tangSelect.trim()}.`

        if (tang) {
            $("#oNameID").val(textO)
            //$('#oRong').val(tang.rong);
            //$('#oCao').val(tang.cao);
        }
    }
}

// Hàm disable/enable sections
function toggleSectionState() {
    if (warehouseData.days.length === 0) {
        $('#keSection .card-body').addClass('disabled-section');
    } else {
        $('#keSection .card-body').removeClass('disabled-section');
    }

    if (warehouseData.kes.length === 0) {
        $('#tangSection .card-body').addClass('disabled-section');
    } else {
        $('#tangSection .card-body').removeClass('disabled-section');
    }

    if (warehouseData.tangs.length === 0) {
        $('#oSection .card-body').addClass('disabled-section');
    } else {
        $('#oSection .card-body').removeClass('disabled-section');

    }
}
function showConfirmDeleteModal(itemName, onConfirm) {
    // Hiển thị tên item trong modal
    $('#confirmDeleteMessage').text(`Bạn có muốn xóa ${itemName} không?`);

    // Bỏ sự kiện cũ để tránh bị gọi nhiều lần
    $('#confirmDeleteBtn').off('click');

    // Khi nhấn Đồng ý
    $('#confirmDeleteBtn').on('click', function () {
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
        // Ẩn modal sau khi xử lý
        const modalEl = document.getElementById('confirmDeleteModal');
        const modal = bootstrap.Modal.getInstance(modalEl);
        modal.hide();
    });

    // Hiện modal
    const modal = new bootstrap.Modal(document.getElementById('confirmDeleteModal'));
    modal.show();
}
function showConfirmCopyModal(itemName, onConfirm) {
    // Hiển thị tên item trong modal
    $('#confirmCopyMessage').text(`${itemName} này đã có dữ liệu. Bạn có muốn copy tiếp không?`);

    // Bỏ sự kiện cũ để tránh bị gọi nhiều lần
    $('#confirmCopyBtn').off('click');

    // Khi nhấn Đồng ý
    $('#confirmCopyBtn').on('click', function () {
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
        // Ẩn modal sau khi xử lý
        const modalEl = document.getElementById('confirmCopy');
        const modal = bootstrap.Modal.getInstance(modalEl);
        modal.hide();
    });

    // Hiện modal
    const modal = new bootstrap.Modal(document.getElementById('confirmCopy'));
    modal.show();
}
var checkComfirm;
var idDayCopy;
var idKeCopy;
var idTangCopy;
function CheckCopyDay(idDay, tenDay) {
    var data = warehouseData.kes.filter(x => x.dayId === idDay)
    if (data.length == 0) {
        showToast('warning', `${tenDay} chưa có dữ liệu!`);
    } else {
        checkComfirm = 1

        const modal = new bootstrap.Modal(document.getElementById('copyModal'));
        modal.show();
        $("#copyInput").val(tenDay)
        $(".copytext").text("Dãy cần copy")
        //$(".pasttext").text("Chọn dãy cần copy")
        idDayCopy = idDay
        const dataKeSelect = warehouseData.days.filter(x => x.id !== idDay)
        let html = ``
        dataKeSelect.map(x => {
            html += `<option value="${x.id}">${x.name}</option>`
        })
        $(".kecopy").hide()
        $(".tangcopy").hide()
        $("#selectToCopy").html(html)
        $("#selectToCopy").val("")
    }
}
function CheckCopyKe(idKe, tenKe, dayId, tenDay) {
    var data = warehouseData.tangs.filter(x => x.keId === idKe)
    if (data.length == 0) {
        showToast('warning', `${tenKe} chưa có dữ liệu!`);
    } else {
        checkComfirm = 2
        const modal = new bootstrap.Modal(document.getElementById('copyModal'));
        modal.show();
        $("#copyInput").val(tenDay)
        $("#copyInputKe").val(tenKe)
        idKeCopy = idKe
        const dataKeSelect = warehouseData.kes.filter(x => x.id !== idKe)
        var listDay = [
            ...new Map(dataKeSelect.map((item) =>
                [`${item.dayId}-${item.dayName}`, { dayId: item.dayId, dayName: item.dayName }]
            )).values()
        ];

        let html = ``
        listDay.map(x => {
            html += `<option  value="${x.dayId}">${x.dayName}</option>`
        })


        $("#selectToCopy").html(html)
        $("#selectToCopy").val(dayId).trigger("change")


        $(".kecopy").show()
        $(".tangcopy").hide()
        renderSelect2Ke("")
    }
}
function renderSelect2Ke(value) {
    var dataKeSelect;
    if (checkComfirm == 2)
        dataKeSelect = warehouseData.kes.filter(x => x.id !== idKeCopy && x.dayId == $("#selectToCopy").val())
    else if (checkComfirm == 3) {
        var data = warehouseData.tangs.filter(x => x.id !== idTangCopy && x.dayId == $("#selectToCopy").val())
        dataKeSelect = [
            ...new Map(data.map((item) =>
                [`${item.keId}-${item.keName}`, { id: item.keId, name: item.keName }]
            )).values()
        ];
    }
    let htmlKe = ``
    dataKeSelect?.map(x => {
        htmlKe += `<option  value="${x.id}">${x.name}</option>`
    })
    $("#selectToCopyKe").html(htmlKe)
    $("#selectToCopyKe").val(value).trigger("change")
    renderSelect2Tang()
}

function renderSelect2Tang(value) {
    const dataTangSelect = warehouseData.tangs.filter(x => x.id !== idTangCopy && x.dayId == $("#selectToCopy").val() && x.keId == $("#selectToCopyKe").val())

    let htmlTang = ``
    dataTangSelect.map(x => {
        htmlTang += `<option  value="${x.id}">${x.name}</option>`
    })
    $("#selectToCopyTang").html(htmlTang)
    $("#selectToCopyTang").val(value).trigger("change")
}
function CheckCopyTang(tangid, tentang, dayid, dayname, keid, kename) {
    var data = warehouseData.os.filter(x => x.tangId == tangid)
    if (data.length == 0) {
        showToast('warning', `${tentang} chưa có dữ liệu!`);
        return
    }
    checkComfirm = 3
    const modal = new bootstrap.Modal(document.getElementById('copyModal'));
    modal.show();
    $("#copyInput").val(dayname)
    $("#copyInputKe").val(kename)
    $("#copyInputTang").val(tentang)
    idTangCopy = tangid
    console.log(warehouseData.tangs)
    const dataKeSelect = warehouseData.tangs.filter(x => x.id !== tangid)

    var listDay = [
        ...new Map(dataKeSelect.map((item) =>
            [`${item.dayId}-${item.dayName}`, { dayId: item.dayId, dayName: item.dayName }]
        )).values()
    ];
    console.log(listDay)
    let html = ``
    listDay.map(x => {
        html += `<option  value="${x.dayId}">${x.dayName}</option>`
    })


    $("#selectToCopy").html(html)
    $("#selectToCopy").val(dayid).trigger("change")

    renderSelect2Ke(keid)
    $(".kecopy").show()
    $(".tangcopy").show()
}
function comfirmCopy() {
    if (checkComfirm == 1) {
        if ($("#selectToCopy").val() == "") {
            showToast('warning', `Vui lòng chọn dẫy cần copy!`);
            return
        }
        var dataKe = warehouseData.kes.filter(x => x.dayId === $("#selectToCopy").val())
        if (dataKe.length > 0) {
            showConfirmCopyModal($("#selectToCopy option:selected").text(), function () {
                var dataKePast = warehouseData.kes.filter(x => x.dayId !== $("#selectToCopy").val())
                var arrDelete = []
                let objectDelete = {
                    DayID: $("#selectToCopy").val()
                }
                arrDelete.push(objectDelete)
                Delete(arrDelete, 1, 2)
                warehouseData.kes = dataKePast
                comfirmCopyDay($("#selectToCopy").val())

            })
        } else {
            comfirmCopyDay($("#selectToCopy").val())
        }

    }
    if (checkComfirm == 2) {
        if (!$("#selectToCopyKe").val()) {
            showToast('warning', `Vui lòng chọn kệ cần copy!`);
            return
        }
        var dataKe = warehouseData.tangs.filter(x => x.keId === $("#selectToCopyKe").val())
        if (dataKe.length > 0) {
            showConfirmCopyModal($("#selectToCopyKe option:selected").text(), function () {
                var dataTangsPast = warehouseData.tangs.filter(x => x.keId !== $("#selectToCopyKe").val())
                var arrDelete = []
                let objectDelete = {
                    KeID: $("#selectToCopyKe").val()
                }
                arrDelete.push(objectDelete)
                Delete(arrDelete, 2, 2)
                warehouseData.tangs = dataTangsPast
                comfirmCopyKe($("#selectToCopyKe").val())
            })
        } else {
            comfirmCopyKe($("#selectToCopyKe").val())
        }
    }
    if (checkComfirm == 3) {
        if (!$("#selectToCopyTang").val()) {
            showToast('warning', `Vui lòng chọn tầng cần copy!`);
            return
        }
        var dataTang = warehouseData.os.filter(x => x.tangId === $("#selectToCopyTang").val())

        if (dataTang.length > 0) {
            showConfirmCopyModal($("#selectToCopyTang option:selected").text(), function () {


                var arrDelete = []
                let objectDelete = {
                    TangID: $("#selectToCopyTang").val()
                }
                arrDelete.push(objectDelete)
                Delete(arrDelete, 3, 2)
                comfirmCopyTang($("#selectToCopyTang").val())
            })
        } else {
            comfirmCopyTang($("#selectToCopyTang").val())
        }
    }
}
async function comfirmCopyTang(valueTangId) {
    var dataOs = warehouseData.os.filter(x => x.tangId == idTangCopy)

    let arrO = []
    var daySelect = $('#selectToCopy option:selected').text().replace("Dãy", "")
    var keSelect = $('#selectToCopyKe option:selected').text().replace("Kệ", "")
    var tangSelect = $('#selectToCopyTang option:selected').text().replace("Tầng", "")
    var textO = `${daySelect.trim()}.${keSelect.trim()}.${tangSelect.trim()}.`

    dataOs.map(x => {
        var oID = generateId()
        var oname = x.name.split(".")[3]
        let object = {
            ID: 0,
            OID: oID,
            TenO: `${textO}${oname}`,
            DayID: $("#selectToCopy").val(),
            KeID: $("#selectToCopyKe").val(),
            TangID: $("#selectToCopyTang").val(),
            Dai: x.dai,
            Cao: x.cao,
            Rong: x.rong,
            NguoiTao: dataUser,

        }
        arrO.push(object)
    })
    keClick = $("#selectToCopyKe").val()
    tangClick = $("#selectToCopyTang").val()
    try {
        const checkSave = await Save(arrO, 4)
        if (checkSave) {
            await GetWarehouseData()
            //arrO.map(x => {
            //    const oData = {
            //        id: generateId(),
            //        tangId: x.TangID,
            //        name: x.TenO,
            //        dai: x.Dai,
            //        rong: x.Rong,
            //        cao: x.Cao,
            //        tangName: $("#selectToCopyTang option:selected").text(),
            //        dayId: $("#selectToCopy").val(),
            //        keId: $("#selectToCopyKe").val(),
            //    }
            //    warehouseData.os.push(oData)
            //})
            showToast("success", "Copy thành công")
            let $tr = $(`#tbodyDay tr[data-id="${$("#selectToCopy").val()}"]`);
            $tr.trigger("click")
        }
        else {
            showToast("error", "Copy thất bại")
        }
    } catch (error) {
        console.log(error)
        showToast("error", "Copy thất bại")
    }
}
async function comfirmCopyKe(valueKeId) {
    var dataTangs = warehouseData.tangs.filter(x => x.keId == idKeCopy)
    let arrTang = []
    dataTangs.map(x => {
        var tangID = generateId()
        const keNPL = {
            ID: 0,
            TangID: tangID,
            TenTang: x.name,
            DayID: $("#selectToCopy").val(),
            KeID: valueKeId,
            Cao: 0,
            Rong: 0,
            Dai: 0,
            NguoiTao: dataUser
        };
        arrTang.push(keNPL)
    })
    keClick = valueKeId
    try {
        const checkSave = await Save(arrTang, 3)
        if (checkSave) {
            arrTang.map(x => {
                const tangData = {
                    id: x.TangID,
                    keId: x.KeID,
                    keName: $("#selectToCopyKe option:selected").text(),
                    name: x.TenTang,
                    dai: x.Dai,
                    rong: x.Rong,
                    cao: x.Cao,
                    dayId: x.DayID,
                    dayName: $("#selectToCopy option:selected").text(),
                };
                warehouseData.tangs.push(tangData)
            })
            updateDropdowns()
            showToast("success", "Copy thành công")
            let $tr = $(`#tbodyDay tr[data-id="${$("#selectToCopy").val()}"]`);
            $tr.trigger("click")

        }
        else {
            showToast("error", "Copy thất bại")
        }
    } catch (error) {
        console.log(error)
        showToast("error", "Copy thất bại")
    }
}
async function comfirmCopyDay(valueDayId) {
    var dataKes = warehouseData.kes.filter(x => x.dayId == idDayCopy)
    let arrKe = []

    dataKes.map(x => {
        var keID = generateId()
        const keNPL = {
            ID: 0,
            KeID: keID,
            TenKe: x.name,
            DayID: valueDayId,
            Dai: 0,
            Rong: 0,
            Cao: 0,
            NguoiTao: dataUser,
            TextKe: x.textKe
        };
        arrKe.push(keNPL)
    })
    try {
        const checkSave = await Save(arrKe, 2)
        if (checkSave) {
            arrKe.map(x => {
                const keData = {
                    id: x.ID,
                    dayId: x.DayID,
                    id: x.KeID,
                    name: x.TenKe,
                    dayName: $("#selectToCopy option:selected").text(),
                    name: x.TenKe,
                    dai: x.Dai,
                    rong: x.Rong,
                    cao: x.Cao,
                    textKe: x.TextKe
                };
                warehouseData.kes.push(keData)
            })
            updateDropdowns()
            showToast("success", "Copy thành công")
            let $tr = $(`#tbodyDay tr[data-id="${valueDayId}"]`);
            $tr.trigger("click")
        }
        else {
            showToast("error", "Copy thất bại")
        }
    } catch (error) {
        console.log(error)
        showToast("error", "Copy thất bại")
    }
}

function renderDay(data) {
    console.log(data)
    $("#tbodyDay").dxDataGrid({
        dataSource: data,
        keyExpr: "id",
        columns: [
            {
                dataField: "name",
                caption: "Dãy",
                minWidth: 100,
                cssClass: "col-header",
                cellTemplate: function (container, options) {
                    const status = options.data.status;
                    const name = options.data.name;

                    let badge = '';
                    if (status == 1) {
                        badge = '<span class="badge bg-success ms-2">Hàng đạt</span>';
                    } else if (status == 2) {
                        badge = '<span class="badge bg-danger ms-2">Hàng lỗi</span>';
                    }

                    $('<div>')
                        .html(name + badge)
                        .appendTo(container);
                }
            },
            {
                caption: "Thao tác",
                width: 80,
                cssClass: "col-header",
                cellTemplate: function (container, options) {
                    // Tạo div bọc
                    const $wrapper = $("<div>")
                        .addClass("d-flex justify-content-center gap-1") // căn giữa + khoảng cách
                        .appendTo(container);

                    // Nút Copy
                    $("<button>")
                        .addClass("btn btn-success item_copy btn-xs")
                        .html('<i class="fas fa-copy"></i>')
                        .on("click", function (e) {
                            e.stopPropagation();
                            /* console.log("Copy row id:", options.data.id);*/
                            CheckCopyDay(options.data.id, options.data.name)
                        })
                        .appendTo($wrapper);

                    // Nút Delete
                    $("<button>")
                        .addClass("btn btn-danger item_delete btn-xs")
                        .html('<i class="fas fa-trash"></i>')
                        .on("click", function (e) {
                            e.stopPropagation();
                            showConfirmDeleteModal(options.data.name, async function () {
                                var arrDay = []
                                let object = {
                                    DayID: options.data.id
                                }
                                arrDay.push(object)
                                var dataDeleteDay = warehouseData.days.filter(x => x.id !== options.data.id)
                                var dataDeleteKe = warehouseData.kes.filter(x => x.dayId !== options.data.id)
                                var dataDeleteTang = warehouseData.tangs.filter(x => x.dayId !== options.data.id)
                                var dataDeleteO = warehouseData.os.filter(x => x.dayId !== options.data.id)
                                try {
                                    const checkDelete = await Delete(arrDay, 1, 1)
                                    if (checkDelete) {
                                        warehouseData.days = dataDeleteDay
                                        warehouseData.kes = dataDeleteKe
                                        warehouseData.tangs = dataDeleteTang
                                        warehouseData.os = dataDeleteO
                                        showToast("success", "Xóa thành công!")
                                        renderDay(warehouseData.days)
                                        updateDropdowns();
                                    } else {
                                        showToast("error", "Xóa thất bại")
                                    }
                                } catch (error) {
                                    console.log(error)
                                }
                            });
                        })
                        .appendTo($wrapper);
                }
            }
        ],

        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },

        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                e.rowElement.attr("data-id", e.data.id);
                if (e.rowIndex === 0) {

                }
                if (e.data.id == dayClick) {
                    $("#tbodyKe .dx-data-row").removeClass("active-row");
                    e.rowElement.addClass("active-row");
                    renderKe(warehouseData.kes, e.data.id)
                    dayClick = null
                } else if (!dayClick && e.rowIndex == 0) {
                    e.rowElement.addClass("active-row");
                    renderKe(warehouseData.kes, e.data.id)
                }
            }
        },
        // Bắt sự kiện click row
        onRowClick: function (e) {
            renderKe(warehouseData.kes, e.data.id)
            $("#tbodyDay .dx-data-row").removeClass("active-row");
            $(e.rowElement).addClass("active-row");
        },

        showBorders: true,
        columnAutoWidth: true,
        wordWrapEnabled: true,
        noDataText: "Không có dữ liệu",
        paging: {
            enabled: false
        },
        scrolling: {
            mode: "standard"
        },
        filterRow: {
            visible: true
        },
        headerFilter: {
            visible: false
        }
    });

    $("#Layer_1").click();
}

function renderKe(data, value) {
    var data = data.filter(x => x.dayId == value)
    if (data.length == 0) {
        renderTang([])
    }
    $("#tbodyKe").dxDataGrid({
        dataSource: data,
        keyExpr: "id",
        columns: [
            {
                dataField: "name",
                caption: "Kệ",
                cssClass: "col-header"
            },
            {
                dataField: "textKe",
                caption: "Tên kệ",
                cssClass: "col-header",
                width: 170,
                cellTemplate: function (container, options) {
                    const value = options.data.textKe
                    console.log(value)
                    $("<div class='position:relative'>")
                        .css({
                            width: "100%",
                            position: "relative",
                            paddingRight: "15px"
                        })
                        .append(
                            $("<input>", {
                                type: "text",
                                readonly: true,
                                value: value,
                                class: "",
                                style: "border:none;outline:none;text-align:center;background:transparent;width:100%;",

                            })
                        )
                        .append(`<i class="fa-solid fa-pen-to-square editKe position-absolute"></i>`)
                        .appendTo(container);
                },
            },
            {
                dataField: "dayName",
                caption: "Dãy",
                cssClass: "col-header"
            },

            {
                dataField: "dai",
                caption: "Dài",
                width: 100,
                cssClass: "col-header",
                calculateCellValue: function (rowData) {
                    return parseFloat(parseFloat(rowData.dai).toFixed(2));
                }
            },
            {
                dataField: "rong",
                caption: "Rộng",
                width: 100,
                cssClass: "col-header",
                 calculateCellValue: function (rowData) {
                     return parseFloat(parseFloat(rowData.rong).toFixed(2));
                }
            },
            {
                dataField: "cao",
                caption: "Cao",
                width: 100,
                cssClass: "col-header",
                calculateCellValue: function (rowData) {
                    return parseFloat(parseFloat(rowData.cao).toFixed(2));
                }
            },
            {
                caption: "Thao tác",
                width: 80,
                cssClass: "col-header",
                cellTemplate: function (container, options) {
                    // Tạo div bọc
                    const $wrapper = $("<div>")
                        .addClass("d-flex justify-content-center gap-1") // căn giữa + khoảng cách
                        .appendTo(container);

                    // Nút Copy
                    $("<button>")
                        .addClass("btn btn-success item_copy btn-xs d-none")
                        .html('<i class="fas fa-copy"></i>')
                        .on("click", function (e) {
                            e.stopPropagation();
                            CheckCopyKe(options.data.id, options.data.name, options.data.dayId, options.data.dayName)
                        })
                        .appendTo($wrapper);

                    // Nút Delete
                    $("<button>")
                        .addClass("btn btn-danger item_delete btn-xs")
                        .html('<i class="fas fa-trash"></i>')
                        .on("click", function (e) {
                            e.stopPropagation();
                            showConfirmDeleteModal(options.data.name, async function () {
                                var arrDay = []
                                let object = {
                                    KeID: options.data.id
                                }
                                arrDay.push(object)

                                var dataDeleteKe = warehouseData.kes.filter(x => x.id !== options.data.id)
                                var dataDeleteTang = warehouseData.tangs.filter(x => x.keId !== options.data.id)
                                var dataDeleteO = warehouseData.os.filter(x => x.keId !== options.data.id)
                                try {
                                    const checkDelete = await Delete(arrDay, 2, 1)
                                    if (checkDelete) {
                                        warehouseData.kes = dataDeleteKe
                                        warehouseData.tangs = dataDeleteTang
                                        warehouseData.os = dataDeleteO
                                        showToast("success", "Xóa thành công!")
                                        renderKe(warehouseData.kes, options.data.dayId)
                                        updateDropdowns();
                                    } else {
                                        showToast("error", "Xóa thất bại")
                                    }
                                } catch (error) {
                                    console.log(error)
                                }
                            });
                        })
                        .appendTo($wrapper);
                }
            }
        ],

        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },

        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                e.rowElement.attr("data-id", e.data.id);
                e.rowElement.attr("data-dayid", e.data.dayId);
                if (e.data.id == keClick) {
                    $("#tbodyKe .dx-data-row").removeClass("active-row");
                    $(e.rowElement).addClass("active-row");
                    renderTang(warehouseData.tangs, e.data.id, e.data.dayId)
                    keClick = null
                } else if (!keClick && e.rowIndex == 0) {
                    e.rowElement.addClass("active-row");
                    renderTang(warehouseData.tangs, e.data.id, e.data.dayId)
                }
            }
        },

        onRowClick: function (e) {
            if ($(e.event.target).closest(".editKe").length > 0) {
                EditNameKe(e.data.id, e.data.textKe)
                return;
            }
            renderTang(warehouseData.tangs, e.data.id, e.data.dayId)
            $("#tbodyKe .dx-data-row").removeClass("active-row");
            $(e.rowElement).addClass("active-row");
        },

        showBorders: true,
        columnAutoWidth: true,
        wordWrapEnabled: true,
        noDataText: "Không có dữ liệu",
        paging: {
            enabled: false
        },
        scrolling: {
            mode: "standard"
        },
        filterRow: {
            visible: true
        },
        headerFilter: {
            visible: false
        }
    });

}


function renderO(data, valueDay, valueKe, valueO) {

}
async function SaveDay() {
    const dayName = $('#dayName').val().trim()
    const dayNameID = $('#dayNameID').val().trim()

    if (!dayName) {
        showToast('error', 'Vui lòng nhập tên dãy!');
        return;
    }

    if (warehouseData.days.some(day => day.name.toUpperCase() === `${dayNameID} ${dayName}`.toUpperCase())) {
        showToast('error', 'Tên dãy đã tồn tại!');
        return;
    }
    const selectedWarehouse = $('input[name="warehouseType"]:checked').val();

    const dayData = {
        id: generateId(),
        name: `${dayNameID} ${dayName}`,
        status: selectedWarehouse
    };
    let arrDay = []
    const dayNPL = {
        iIDd: 0,
        DayID: dayData.id,
        TenDay: dayData.name,
        NguoiTao: dataUser,
        Module: checkWarehouse,
        Status: selectedWarehouse
    };
    arrDay.push(dayNPL)

    try {
        const checkSave = await Save(arrDay, 1);

        if (checkSave) {
            // Chỉ thực hiện khi lưu thành công
            warehouseData.days.push(dayData);
            $('#dayNameID').val('Dãy');
            $('#dayName').val('');
            currentSelectedDay = dayData.id;

            updateDropdowns();
            toggleSectionState();
            showToast('success', `Đã tạo dãy "${dayName}" thành công và tự động chọn!`);
            renderDay(warehouseData.days);
        } else {
            showToast('error', 'Lưu dãy thất bại!');
        }
    } catch (error) {
        console.error('Lỗi khi lưu dãy:', error);
        showToast('error', 'Có lỗi xảy ra khi lưu dãy!');
    }
}
async function SaveKe() {
    const selectedDayId = $('#selectedDay').val()
    const selectedDayText = $('#selectedDay option:selected').text()
    const keNameID = $('#keNameID').val().trim();
    const keName = $('#keName').val().trim();
    const keDai = $('#keDai').val();
    const keRong = $('#keRong').val();
    const keCao = $('#keCao').val();

    if (!selectedDayId) {
        showToast('error', 'Vui lòng chọn dãy!');
        return;
    }

    if (!keName) {
        showToast('error', 'Vui lòng nhập mã kệ!');
        return;
    }

    if (!keDai || !keRong || !keCao) {
        showToast('error', 'Vui lòng nhập đầy đủ kích thước kệ!');
        return;
    }

    const existingKe = warehouseData.kes.find(ke =>
        ke.dayId === selectedDayId && ke.name.toUpperCase() === `${keNameID} ${keName}`.toUpperCase()
    );
    if (existingKe) {
        showToast('error', 'Tên kệ đã tồn tại trong dãy này!');
        return;
    }

    const keData = {
        id: generateId(),
        dayId: selectedDayId,
        dayName: selectedDayText,
        name: `${keNameID} ${keName}`,
        dai: parseFloat(keDai),
        rong: parseFloat(keRong),
        cao: parseFloat(keCao),
        textKe: $('#keText').val().trim()
    };
    let arrKe = []
    const keNPL = {
        ID: 0,        // not null
        KeID: keData.id,   // nullable
        TenKe: keData.name,  // nullable
        DayID: keData.dayId,  // nullable
        Dai: keData.dai,    // nullable
        Rong: keData.rong,   // nullable
        Cao: keData.cao,    // nullable
        NguoiTao: dataUser,
        TextKe: keData.textKe,
        Module: checkWarehouse

    };
    arrKe.push(keNPL)
    try {
        const checkSave = await Save(arrKe, 2)
        if (checkSave) {
            dayClick = $("#selectedDay").val()
            await GetWarehouseData()
            //warehouseData.kes.push(keData);

            // TỰ ĐỘNG CHỌN KỆ VỪA TẠO
            currentSelectedKe = keData.id;

            $('#keNameID').val('Kệ');
            $('#keName').val('');
            $('#keText').val('');
            $('#keDai').val(0);
            $('#keRong').val(0);
            $('#keCao').val(0);

            updateDropdowns();
            toggleSectionState();
            showToast('success', `Đã tạo kệ ${keName} - "${$('#keText').val().trim()}" thành công và tự động chọn!`);

            /*  renderKe(warehouseData.kes, $("#selectedDay").val())*/
        } else {
            showToast('error', 'Lưu kệ thất bại!');
        }
    } catch (error) {
        console.error('Lỗi khi lưu kệ:', error);
        showToast('error', 'Có lỗi xảy ra khi lưu tầng!');
    }
}
async function SaveTang() {
    const selectedKeId = $('#selectedKe').val();
    const selectedKeText = $('#selectedKe option:selected').text();
    const tangSoLuong = parseInt($('#tangSoLuong').val());
    const tangText = $('#tangText').val().trim(); // Tên tùy chọn

    // Validate
    if (!selectedKeId) {
        showToast('error', 'Vui lòng chọn kệ!');
        return;
    }

    if (!tangSoLuong || tangSoLuong < 1) {
        showToast('error', 'Vui lòng nhập số lượng tầng (tối thiểu 1)!');
        return;
    }


    // Kiểm tra tầng đã tồn tại
    const existingTangs = warehouseData.tangs.filter(tang => tang.keId === selectedKeId);

    // Tìm số tầng lớn nhất hiện có
    let maxTangNumber = 0;
    existingTangs.forEach(tang => {
        const match = tang.name.match(/\d+$/);
        if (match) {
            const num = parseInt(match[0]);
            if (num > maxTangNumber) maxTangNumber = num;
        }
    });

    // Bắt đầu từ tầng tiếp theo
    const startNumber = maxTangNumber + 1;

    // Chuẩn bị mảng dữ liệu để lưu
    let arrTang = [];

    for (let i = 0; i < tangSoLuong; i++) {
        const tangNumber = startNumber + i;
        const tangName = `Tầng ${tangNumber}`;

        // Kiểm tra trùng lặp (phòng trường hợp)
        const isDuplicate = warehouseData.tangs.some(tang =>
            tang.keId === selectedKeId && tang.name.toUpperCase() === tangName.toUpperCase()
        );

        if (isDuplicate) {
            showToast('warning', `Tầng ${tangNumber} đã tồn tại, bỏ qua!`);
            continue;
        }

        const tangData = {
            id: generateId(),
            keId: selectedKeId,
            keName: selectedKeText,
            name: tangName,
            dai: 0,
            rong: 0,
            cao: 0,
            dayId: $('#selectedDay').val(),
            dayName: $('#selectedDay option:selected').text(),
            textTang: tangText || "" // Dùng tên tùy chọn hoặc mặc định
        };

        const tangNPL = {
            ID: 0,
            TangID: tangData.id,
            TenTang: tangData.name,
            DayID: tangData.dayId,
            KeID: tangData.keId,
            Cao: tangData.cao,
            Rong: tangData.rong,
            Dai: tangData.dai,
            NguoiTao: dataUser,
            TextTang: tangData.textTang,
            Module:checkWarehouse
        };

        arrTang.push(tangNPL);
    }

    if (arrTang.length === 0) {
        showToast('warning', 'Không có tầng nào được tạo!');
        return;
    }

    // Lưu vào database
    try {
        const checkSave = await Save(arrTang, 3);

        if (checkSave) {
            dayClick = $('#selectedDay').val();
            keClick = selectedKeId;
            await GetWarehouseData();

            // Tự động chọn tầng cuối cùng vừa tạo
            currentSelectedTang = arrTang[arrTang.length - 1].TangID;

            // Reset input
            $('#tangSoLuong').val('');
            $('#tangText').val('');

            updateDropdowns();
            toggleSectionState();

            showToast('success', `Đã tạo ${arrTang.length} tầng thành công (từ Tầng ${startNumber} đến Tầng ${startNumber + arrTang.length - 1})!`);
        } else {
            showToast('error', 'Lưu tầng thất bại!');
        }
    } catch (error) {
        console.error('Lỗi khi lưu tầng:', error);
        showToast('error', 'Có lỗi xảy ra khi lưu tầng!');
    }
}
async function SaveO() {
    const selectedTangId = $('#selectedTang').val();
    const oNameID = $('#oNameID').val().trim();
    const oName = $('#oName').val().trim();
    const oDai = $('#oDai').val();
    const oRong = $('#oRong').val();
    const oCao = $('#oCao').val();

    if (!selectedTangId) {
        showToast('error', 'Vui lòng chọn tầng!');
        return;
    }
    if (!oName) {
        showToast('error', 'Vui lòng nhập tên ô!');
        return;
    }
    if (!oDai) {
        showToast('error', 'Vui lòng nhập chiều dài ô!');
        return;
    }

    const existingO = warehouseData.os.find(o =>
        o.tangId === selectedTangId && o.name.toUpperCase() === `${oNameID}${oName}`.toUpperCase()
    );
    if (existingO) {
        showToast('error', 'Tên ô đã tồn tại trong tầng này!');
        return;
    }

    const selectedTang = warehouseData.tangs.find(t => t.id === selectedTangId);
    const selectedKe = warehouseData.kes.find(k => k.id === $('#selectedKe').val());

    const oData = {
        id: generateId(),
        tangId: selectedTangId,
        name: `${oNameID}${oName}`,
        dai: parseFloat(oDai),
        rong: parseFloat(oRong) || 0,
        cao: parseFloat(oCao) || 0,
        tangName: $('#selectedTang option:selected').text(),
        dayId: $('#selectedDay').val(),
        keId: $('#selectedKe').val(),
        textO: $('#oText').val().trim()
    }

    let arrO = []
    const oNPL = {
        ID: 0,
        OID: oData.id,
        TenO: oData.name,
        DayID: oData.dayId,
        KeID: oData.keId,
        TangID: oData.tangId,
        Dai: oData.dai,
        Cao: oData.cao,
        Rong: oData.rong,
        NguoiTao: dataUser,
        TextO: $('#oText').val().trim(),
        Module:checkWarehouse
    };
    arrO.push(oNPL)

    try {
        const checkSave = await Save(arrO, 4)
        if (checkSave) {
            dayClick = $("#selectedDay").val()
            keClick = $("#selectedKe").val()
            tangClick = $('#selectedTang').val()
            await GetWarehouseData()


            // Reset input
            $('#oName').val('');
            $('#oDai').val('');
            $('#oRong').val('');
            $('#oCao').val('');

            showToast('success', `Đã tạo ${$('#oText').val().trim()} - ${oNameID}${oName} thành công!`);

            // Render lại Kệ, Tầng, và Ô
            //renderKe(warehouseData.kes, $("#selectedDay").val());

            //renderTang(warehouseData.tangs, $("#selectedKe").val(), $("#selectedDay").val());
            //renderO(warehouseData.os, $("#selectedDay").val(), $("#selectedKe").val(), $('#selectedTang').val());
        } else {
            showToast('error', 'Lưu ô thất bại!');
        }
    } catch (error) {
        console.error('Lỗi khi lưu ô:', error);
        showToast('error', 'Có lỗi xảy ra khi lưu ô!');
    }
}
async function Save(arrSave, value) {
    try {
        const request = new Request(`/api/ViTriKhoNPL/Post?para1=${value}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(arrSave),
        });

        const response = await fetch(request);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        return data === "True";
    } catch (error) {
        console.error('Error in Save function:', error);
        return false;
    }
}
async function Delete(arrDelete, value, valueDelete) {
    try {
        const request = new Request(`/api/ViTriKhoNPL/Delete?para1=${value}&para2=${valueDelete}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(arrDelete),
        });

        const response = await fetch(request);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        return data === "True";
    } catch (error) {
        console.error('Error in Save function:', error);
        return false;
    }
}
async function GetWarehouseData(value) {
    const url = `/api/ViTriKhoNPL/Get?action=Get&para1=${checkWarehouse}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        var dayData = data.Table
         renderKe([])
        warehouseData = {
            days: [],
            kes: [],
            tangs: [],
            os: []
        };
        if (data.Table1)
            await dataKe(data.Table1)
        if (data.Table2)
            await dataTang(data.Table2)
        if (data.Table3)
            await dataO(data.Table3)
        if (data.Table)
            await dataDay(dayData, value)

    } catch (error) {
        console.error(error.message);
    }
}
function dataDay(data, value) {
    data.map(x => {
        const dayData = {
            id: x.DayID,
            name: x.TenDay,
            status: x.Status,
        };
        warehouseData.days.push(dayData)
    })
    updateDropdowns()
    toggleSectionState();
    if (value != 1)
        renderDay(warehouseData.days)
}
function dataKe(data) {

    data.map(x => {
        const keData = {
            id: x.KeID,
            dayId: x.DayID,
            dayName: x.TenDay,
            name: x.TenKe,
            dai: parseFloat(x.Dai),
            rong: parseFloat(x.Rong),
            cao: parseFloat(x.Cao),
            textKe: x.TextKe
        };
        warehouseData.kes.push(keData)
    })
    updateDropdowns();
    toggleSectionState();
    /*  renderKe(warehouseData.kes, data[0].DayID)*/
}
function dataTang(data) {
    data.map(x => {
        const tangData = {
            id: x.TangID,
            keId: x.KeID,
            keName: x.TenKe,
            name: x.TenTang,
            dai: parseFloat(x.Dai),
            rong: parseFloat(x.Rong),
            cao: parseFloat(x.Cao),
            dayId: x.DayID,
            dayName: x.TenDay,
            textTang: x.TextTang
        };
        warehouseData.tangs.push(tangData)
    })
    updateDropdowns();
    toggleSectionState();
    /* renderTang(warehouseData.tangs, data[0].KeID)*/
}
function dataO(data) {
    data.map(x => {
        const oData = {
            id: x.OID,
            tangId: x.TangID,
            name: x.TenO,
            dai: x.Dai,
            rong: x.Rong,
            cao: x.Cao,
            tangName: x.TenTang,
            dayId: x.DayID,
            keId: x.KeID,
            textO: x.TextO
        }
        warehouseData.os.push(oData)
    })
}
var idONPL;
function EditO(idO) {
    idONPL = idO;

    const item = warehouseData.os.find(x => x.id === idONPL)
    if (!item) return;

    // đổ dữ liệu vào TABLE
    $("#tblTangName").text(item.tangName);
    $("#tblTenO").text(item.name);
    $("#tblTenTextO").text(item.textO);
    $("#tblDai").text(item.dai);
    $("#tblRong").text(item.rong);
    $("#tblCao").text(item.cao);

    // gán dữ liệu vào INPUT chỉnh sửa
    $("#oId").val(item.id);
    $("#inpTextO").val(item.textO);
    $("#inpDai").val(item.dai);
    $("#inpRong").val(item.rong);
    $("#inpCao").val(item.cao);

    // mở modal
    let modal = new bootstrap.Modal(document.getElementById("editOModel"));
    modal.show();
}
$("#btnSaveO").on("click", async function () {


    const found = warehouseData.os.find(x => x.id === idONPL)
    var arrO = []

    // cập nhật lại object
    found.textO = $("#inpTextO").val();
    found.dai = Number($("#inpDai").val());
    found.rong = Number($("#inpRong").val());
    found.cao = Number($("#inpCao").val());

    const oNPL = {
        ID: 0,
        OID: idONPL,
        TenO: "",
        DayID: found.dayId,
        KeID: found.keId,
        TangID: found.tangId,
        Dai: Number($("#inpDai").val()),
        Cao: Number($("#inpCao").val()),
        Rong: Number($("#inpRong").val()),
        NguoiTao: "",
        TextO: $("#inpTextO").val()
    };
    arrO.push(oNPL)
    const checkSave = await Save(arrO, 4)
    if (checkSave) {
        dayClick = found.dayId
        keClick = found.keId
        tangClick = found.tangId
        await GetWarehouseData()


        // Reset input
        $('#oName').val('');
        $('#oDai').val('');
        $('#oRong').val('');
        $('#oCao').val('');

        showToast('success', `Sửa thông tin thành công!`);


    } else {
        showToast('error', '`Sửa thông tin ô thất bại!');
    }
    // đóng modal
    bootstrap.Modal.getInstance(document.getElementById("editOModel")).hide();
})
var checkEditName = ""
var IDKe = ""
var IDTang = ""
var IDO = ""
function EditNameKe(id, text) {
    $("#editNameInput").val("");
    checkEditName = 1
    IDKe = id
    $("#labelEditName").text("Sửa Tên Kệ");
    $("#editNameInput").val(text);
    new bootstrap.Modal(document.getElementById("editOneNameModal")).show();
}
function EditNameTang(id, text) {
    $("#editNameInput").val("");
    checkEditName = 2
    IDTang = id
    $("#labelEditName").text("Sửa Tên Tầng");
    $("#editNameInput").val(text);
    new bootstrap.Modal(document.getElementById("editOneNameModal")).show();
}
function EditNameO(id, text) {
    $("#editNameInput").val("");
    checkEditName = 3
    IDO = id
    console.log(IDO)
    $("#labelEditName").text("Sửa Tên Ô");
    $("#editNameInput").val(text);
    new bootstrap.Modal(document.getElementById("editOneNameModal")).show();
}
function EditNameOCreate(text) {
    $("#editNameInput").val("");
    checkEditName = 4
    $("#labelEditName").text("Sửa Tên Ô");
    $("#editNameInput").val(text);
    new bootstrap.Modal(document.getElementById("editOneNameModal")).show();
}
$("#editOneNameModal").on('shown.bs.modal', function () {
    $("#editNameInput").focus();
});
$("#btnSaveOneName").on("click", function () {
    if (checkEditName == 1) {
        EditKe()
    } else if (checkEditName == 2) EditTang()
    else if (checkEditName == 3) {
        EditO()
    }
    else {
        EditOCreate()
    }
})
async function EditKe() {
    const existingKe = warehouseData.kes.find(ke => ke.id.toUpperCase() == IDKe.toUpperCase());

    let arrKe = []
    const keNPL = {
        ID: 0,        // not null
        KeID: IDKe,   // nullable
        TenKe: existingKe.name,  // nullable
        DayID: existingKe.dayId,  // nullable
        Dai: existingKe.dai,    // nullable
        Rong: existingKe.rong,   // nullable
        Cao: existingKe.cao,    // nullable
        NguoiTao: "",
        TextKe: $("#editNameInput").val(),
    };
    arrKe.push(keNPL)
    try {
        const checkSave = await Save(arrKe, 2)
        if (checkSave) {
            dayClick = existingKe.dayId
            keClick = existingKe.id
            await GetWarehouseData() 

            showToast('success', `Sửa tên kệ thành công!`);
            $("#editOneNameModal").modal("hide")
        } else {
            showToast('error', 'Sửa tên kệ thất bại!');
        }
    } catch (error) {
        console.error('Lỗi khi lưu kệ:', error);
        showToast('error', 'Có lỗi xảy ra khi lưu tầng!');
    }
}
async function EditTang() {
    const existingTang = warehouseData.tangs.find(tang => tang.id.toUpperCase() == IDTang.toUpperCase());
    let arrKe = []
    const tangNPL = {
        ID: 0,          // not null
        TangID: existingTang.id,   // nullable
        TenTang: existingTang.name,  // nullable
        DayID: "",
        KeID: existingTang.keId,     // nullable
        Cao: existingTang.cao,      // nullable
        Rong: existingTang.rong,     // nullable
        Dai: existingTang.dai,       // nullable
        NguoiTao: "",
        TextTang: $("#editNameInput").val(),
    };
    arrKe.push(tangNPL)
    $(`#tbodyTang .${trEditTang}`).find(`.newTextTang`).text($("#editNameInput").val())
    try {
        const checkSave = await Save(arrKe, 3)
        if (checkSave) {

            showToast('success', `Sửa tên tầng thành công!`);
            $("#editOneNameModal").modal("hide")

        } else {
            showToast('error', 'Sửa tên tầng thất bại!');
        }
    } catch (error) {
        console.error('Lỗi khi lưu tầng:', error);
        showToast('error', 'Có lỗi xảy ra khi lưu tầng!');
    }

}
async function EditOCreate() {
    console.log(trEditOCreate)
    $(`#previewOTableBody .${trEditOCreate}`).find(`.newTextO`).text($("#editNameInput").val())
    $("#editOneNameModal").modal("hide")
}
async function EditO() {
    const found = warehouseData.os.find(x => x.id.toUpperCase() == IDO.toUpperCase())
    var arrO = []

    const oNPL = {
        ID: 0,
        OID: IDO,
        TenO: "",
        DayID: found.dayId,
        KeID: found.keId,
        TangID: found.tangId,
        Dai: 0,
        Cao: 0,
        Rong: 0,
        NguoiTao: "",
        TextO: $("#editNameInput").val()
    };
    arrO.push(oNPL)
    $(`#tbodyTang .${trEditO}`).find(`.newTextO`).text($("#editNameInput").val())


    try {
        const checkSave = await Save(arrO, 4)
        if (checkSave) {

            showToast('success', `Sửa tên ô thành công!`);
            $("#editOneNameModal").modal("hide")

        } else {
            showToast('error', 'Sửa tên ô thất bại!');
        }
    } catch (error) {
        console.error('Lỗi khi lưu tầng:', error);
        showToast('error', 'Có lỗi xảy ra khi lưu tầng!');
    }
}
$('#openCreateOModal').click(function () {
    const selectedKeId = $('#selectedKe').val();
    if (!selectedKeId) {
        showToast('error', 'Vui lòng chọn kệ!');
        return;
    }
    $('#allSoLuong').val("");
    $('#allDai').val("");
    $('#allRong').val("");
    $('#allCao').val("");
    $('#previewOTableBody').empty();
    // Lấy danh sách tầng của kệ được chọn
    const tangsOfKe = warehouseData.tangs.filter(tang => tang.keId === selectedKeId);

    if (tangsOfKe.length === 0) {
        showToast('warning', 'Kệ này chưa có tầng nào!');
        return;
    }

    // Render danh sách tầng vào modal
    renderTangCardsForO(tangsOfKe, selectedKeId);

    // Hiển thị modal

    $("#createOModal").modal("show")
});
// Render danh sách tầng
function renderTangCardsForO(tangs, keId) {
    const dayId = $('#selectedDay').val();
    const dayName = $('#selectedDay option:selected').text().replace('Dãy', '').trim();
    const keName = $('#selectedKe2 option:selected').text().replace('Kệ', '').trim();
  
    let html = '';

    tangs.forEach(tang => {
        // Kiểm tra số ô đã có của tầng này
        const existingOs = warehouseData.os.filter(o => o.tangId === tang.id);
        const maxONumber = existingOs.length > 0
            ? Math.max(...existingOs.map(o => {
                const match = o.name.match(/\.(\d+)$/);
                return match ? parseInt(match[1]) : 0;
            }))
            : 0;

        const tangNumber = tang.name.replace('Tầng', '').trim();

        html += `
            <div class="col-md-6 col-lg-3">
                <div class="tang-card" data-tang-id="${tang.id}" data-tang-name="${tang.name}">
                    <div class="tang-card-header">
                        ${tang.name}
                    </div>

                    ${existingOs.length > 0 ? `
                        <div class="existing-info d-none">
                            <i class="fas fa-check-circle"></i> Đã có ${existingOs.length} ô
                        </div>` : ''}

                    <div class="row">
                        <div class="mb-2 col-6">
                            <label>Số lượng ô</label>
                            <input type="number" class="form-control form-control-sm tang-soluong" min="1" value="1">
                        </div>

                        <div class="mb-2 col-6">
                            <label>Dài (m)</label>
                            <input type="number" class="form-control form-control-sm tang-dai" step="0.01" value="1">
                        </div>
                    </div>

                    <div class="row">
                        <div class="mb-2 col-6">
                            <label>Rộng (m)</label>
                            <input type="number" class="form-control form-control-sm tang-rong" step="0.01" value="1">
                        </div>

                        <div class="mb-0 col-6">
                            <label>Cao (m)</label>
                            <input type="number" class="form-control form-control-sm tang-cao" step="0.01" value="1">
                        </div>
                    </div>
                </div>
            </div>
        `;

    });

    $('#tangCardsContainer').html(html);
}
$('#renderDanhSachO').click(function () {
    const cards = $('.tang-card');
    let hasError = false;
    let tangDataList = [];

    // Validate và thu thập dữ liệu
    cards.each(function () {
        const $card = $(this);
        const tangId = $card.data('tang-id');
        const tangName = $card.data('tang-name');

        const soLuong = parseInt($card.find('.tang-soluong').val()) || 0;
        const dai = parseFloat($card.find('.tang-dai').val()) || 0;
        const rong = parseFloat($card.find('.tang-rong').val()) || 0;
        const cao = parseFloat($card.find('.tang-cao').val()) || 0;

        // Validate
        if (soLuong <= 0) {
            showToast('warning', `${tangName}: Số lượng ô phải lớn hơn 0!`);
            hasError = true;
            return false;
        }

        if (dai <= 0) {
            showToast('warning', `${tangName}: Chiều dài phải lớn hơn 0!`);
            hasError = true;
            return false;
        }

        if (rong <= 0) {
            showToast('warning', `${tangName}: Chiều rộng phải lớn hơn 0!`);
            hasError = true;
            return false;
        }

        if (cao <= 0) {
            showToast('warning', `${tangName}: Chiều cao phải lớn hơn 0!`);
            hasError = true;
            return false;
        }

        // Lấy số tầng để sort
        const tangNumber = parseInt(tangName.replace('Tầng', '').trim());

        tangDataList.push({
            tangId: tangId,
            tangName: tangName,
            tangNumber: tangNumber,
            soLuong: soLuong,
            dai: dai,
            rong: rong,
            cao: cao
        });
    });

    if (hasError) return;

    if (tangDataList.length === 0) {
        showToast('warning', 'Không có dữ liệu để hiển thị!');
        return;
    }

    // Sort từ lớn đến nhỏ (tầng cao -> tầng thấp) để hiển thị từ trên xuống
    tangDataList.sort((a, b) => a.tangNumber - b.tangNumber);

    // Render bảng preview dạng lưới
    renderPreviewOGrid(tangDataList);

    // Hiển thị container preview
    $('#previewOContainer').slideDown();
});
function renderPreviewOGrid(tangDataList) {
    const dayName = $('#selectedDay option:selected').text().replace('Dãy', '').trim();
    const keName = $('#selectedKe option:selected').text().replace('Kệ', '').trim();

    let html = '';
    let totalO = 0;

    // Đảo ngược mảng để tầng thấp hiển thị ở dưới
    const orderedTangList = [...tangDataList].reverse();

    // Tìm số cột lớn nhất
    const maxCols = Math.max(...orderedTangList.map(t => t.soLuong));

    // Tạo ma trận đánh số từ DƯỚI LÊN (cột trái sang phải)
    let numberMatrix = [];
    let currentNumber = 1;

    for (let col = 0; col < maxCols; col++) {
        let columnNumbers = [];
        // Duyệt từ cuối mảng (Tầng 1 - dưới cùng) lên đầu mảng (Tầng 4 - trên cùng)
        for (let row = orderedTangList.length - 1; row >= 0; row--) {
            if (col < orderedTangList[row].soLuong) {
                columnNumbers.unshift(currentNumber); // Thêm vào đầu mảng
                currentNumber++;
            } else {
                columnNumbers.unshift(null);
            }
        }
        numberMatrix.push(columnNumbers);
    }


    // Render bảng (từ trên xuống: Tầng 4 → Tầng 1)
    orderedTangList.forEach((tangData, rowIndex) => {
        const tangNumber = tangData.tangNumber;

        const existingOs = warehouseData.os.filter(o => o.tangId === tangData.tangId);
        let maxONumber = 0;
        existingOs.forEach(o => {
            const match = o.name.match(/\.(\d+)$/);
            if (match) {
                const num = parseInt(match[1]);
                if (num > maxONumber) maxONumber = num;
            }
        });

        html += `<tr>`;
        html += `<td class="text-center align-middle" style="background: linear-gradient(45deg, #667eea, #764ba2); color: white; font-weight: bold; width: 80px;">
                    ${tangData.tangName}
                </td>`;

        for (let col = 0; col < tangData.soLuong; col++) {
            const oNumber = maxONumber + col + 1;
            const displayNumber = numberMatrix[col][rowIndex];
            const oName = `${dayName}.${keName}.${tangNumber}.${displayNumber}`;

            const isLastCell = (col === tangData.soLuong - 1);
            const colspan = isLastCell ? (maxCols - tangData.soLuong + 1) : 1;

            html += `
                <td class="text-center o-cell" colspan="${colspan}" style="padding: 8px; min-width: 110px;position: relative;">
                    <div style="font-weight: bold; color: #667eea; margin-bottom: 4px; font-size: 0.9rem;">
                         ${oName}
                    </div>
                    <div class="${dayName}${keName}${tangNumber}${displayNumber}" style="font-size: 0.75rem; color: #333; font-weight: 600;"><span class="newTextO">Ô ${displayNumber}</span> <i style="cursor: pointer;
                            color: #b58080;
                            position: absolute;
                            top: 1px;
                            right: 6px;
                            font-size: 14px;" class="fa-solid fa-pen-to-square editOText"></i>
                    </div>
                    <div style="font-size: 0.7rem; color: #666; margin-top: 2px;">
                        ${tangData.dai} × ${tangData.rong} × ${tangData.cao}
                    </div>
                </td>
            `;
            totalO++;
        }

        html += `</tr>`;
    });

    $('#previewOTableBody').html(html);
    $('#totalOCount').text(totalO);
}

$('#applyAllValues').on('click', function () {
    const allSoLuong = $('#allSoLuong').val();
    const allDai = $('#allDai').val();
    const allRong = $('#allRong').val();
    const allCao = $('#allCao').val();

    let appliedCount = 0;

    // Áp dụng cho tất cả các card tầng
    $('.tang-card').each(function () {
        if (allSoLuong) {
            $(this).find('.tang-soluong').val(allSoLuong);
            appliedCount++;
        }
        if (allDai) {
            $(this).find('.tang-dai').val(allDai);
            appliedCount++;
        }
        if (allRong) {
            $(this).find('.tang-rong').val(allRong);
            appliedCount++;
        }
        if (allCao) {
            $(this).find('.tang-cao').val(allCao);
            appliedCount++;
        }
    });

    if (appliedCount > 0) {
        showToast('success', 'Đã áp dụng giá trị cho tất cả các tầng!');
    } else {
        showToast('warning', 'Vui lòng nhập ít nhất một giá trị!');
    }
});
$('#confirmCreateO').click(async function () {
    const cards = $('.tang-card');
    let tangDataList = [];
    let hasError = false;

    // Thu thập dữ liệu và validate
    cards.each(function () {
        const $card = $(this);
        const tangId = $card.data('tang-id');
        const tangName = $card.data('tang-name');

        const soLuong = parseInt($card.find('.tang-soluong').val()) || 0;
        const dai = parseFloat($card.find('.tang-dai').val()) || 0;
        const rong = parseFloat($card.find('.tang-rong').val()) || 0;
        const cao = parseFloat($card.find('.tang-cao').val()) || 0;

        // Validate
        if (soLuong <= 0) {
            showToast('warning', `${tangName}: Số lượng ô phải lớn hơn 0!`);
            hasError = true;
            return false;
        }

        if (dai <= 0 || rong <= 0 || cao <= 0) {
            showToast('warning', `${tangName}: Kích thước phải lớn hơn 0!`);
            hasError = true;
            return false;
        }

        const tangNumber = parseInt(tangName.replace('Tầng', '').trim());

        tangDataList.push({
            tangId: tangId,
            tangName: tangName,
            tangNumber: tangNumber,
            soLuong: soLuong,
            dai: dai,
            rong: rong,
            cao: cao
        });
    });

    if (hasError) return;

    if (tangDataList.length === 0) {
        showToast('warning', 'Không có ô nào được tạo!');
        return;
    }

    // Sort từ nhỏ đến lớn
    tangDataList.sort((a, b) => a.tangNumber - b.tangNumber);

    // Đảo ngược để tầng thấp ở cuối mảng
    const orderedTangList = [...tangDataList].reverse();

    // Tìm số cột lớn nhất
    const maxCols = Math.max(...orderedTangList.map(t => t.soLuong));

    // Tạo ma trận đánh số từ DƯỚI LÊN (giống render)
    let numberMatrix = [];
    let currentNumber = 1;

    for (let col = 0; col < maxCols; col++) {
        let columnNumbers = [];
        for (let row = orderedTangList.length - 1; row >= 0; row--) {
            if (col < orderedTangList[row].soLuong) {
                columnNumbers.unshift(currentNumber);
                currentNumber++;
            } else {
                columnNumbers.unshift(null);
            }
        }
        numberMatrix.push(columnNumbers);
    }

    // Tạo mảng arrO với tên ô đúng
    let arrO = [];
    const dayId = $('#selectedDay').val();
    const keId = $('#selectedKe').val();
    const dayName = $('#selectedDay option:selected').text().replace('Dãy', '').trim();
    const keName = $('#selectedKe option:selected').text().replace('Kệ', '').trim();

    orderedTangList.forEach((tangData, rowIndex) => {
        const tangNumber = tangData.tangNumber;

        // Tìm số ô lớn nhất hiện có
        const existingOs = warehouseData.os.filter(o => o.tangId === tangData.tangId);
        let maxONumber = 0;
        existingOs.forEach(o => {
            const match = o.name.match(/\.(\d+)$/);
            if (match) {
                const num = parseInt(match[1]);
                if (num > maxONumber) maxONumber = num;
            }
        });

        // Tạo các ô với số thứ tự từ ma trận
        for (let col = 0; col < tangData.soLuong; col++) {
            const displayNumber = numberMatrix[col][rowIndex];
            const oId = generateId();
            const oName = `${dayName}.${keName}.${tangNumber}.${displayNumber}`;
            const classText = `${dayName}${keName}${tangNumber}${displayNumber}`
            const oNPL = {
                ID: 0,
                OID: oId,
                TenO: oName,
                DayID: dayId,
                KeID: keId,
                TangID: tangData.tangId,
                Dai: tangData.dai,
                Cao: tangData.cao,
                Rong: tangData.rong,
                NguoiTao: dataUser,
                TextO: $(`#previewOTableBody .${classText}`).find(`.newTextO`).text(),
                Module:checkWarehouse
            };

            arrO.push(oNPL);
        }
    });
   
    // Lưu vào database
    try {
        const checkSave = await Save(arrO, 4);

        if (checkSave) {
            dayClick = $('#selectedDay').val();
            keClick = $('#selectedKe').val();
            await GetWarehouseData();

            // Đóng modal
            bootstrap.Modal.getInstance(document.getElementById('createOModal')).hide();

            showToast('success', `Đã tạo ${arrO.length} ô thành công!`);
        } else {
            showToast('error', 'Lưu ô thất bại!');
        }
    } catch (error) {
        console.error('Lỗi khi lưu ô:', error);
        showToast('error', 'Có lỗi xảy ra khi lưu ô!');
    }
});
async function renderTang(data1, keId, dayID) {
     keLocal = keId
     dayLocal = dayID
    const url = `/api/ViTriKhoNPL/Get?action=GetTangONPL&para1=${dayID}&para2=${keId}&para3=${checkWarehouse}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        renderViTriKhoGrid(data.Table, dayID, keId);

    } catch (error) {
        console.error(error.message);
    }
}

// Hàm render dữ liệu dạng lưới
function renderViTriKhoGrid(data, dayId, keId) {

    // Kiểm tra nếu không có dữ liệu
    if (!data || data.length === 0) {
        // Lấy dữ liệu tầng từ warehouseData.tangs
        const tangsOfKe = warehouseData.tangs.filter(t => t.keId === keId && t.dayId === dayId);
        if (tangsOfKe.length > 0) {
            renderTangListOnly(tangsOfKe, dayId, keId);
        } else {
            $('#tbodyTang').html('<div class="alert alert-warning text-center">Kệ này chưa có tầng nào</div>');
        }
        return;
    }

    const hasO = data.some(item => item.OID != null && item.OID != '');
    if (!hasO) {
        // Nếu chỉ có tầng, chưa có ô
        const tangsOfKe = warehouseData.tangs.filter(t => t.keId === keId && t.dayId === dayId);
        renderTangListOnly(tangsOfKe, dayId, keId);
        return;
    }

    // Group data theo tầng (có ô)
    const tangGroups = {};
    data.forEach(item => {
        if (!tangGroups[item.TangID]) {
            tangGroups[item.TangID] = {
                tangId: item.TangID,
                tangName: item.TenTang,
                tangNumber: parseInt(item.TenTang.replace('Tầng', '').replace('tầng', '').trim()),
                textTang: item.TextTang || '',
                os: []
            };
        }
        if (item.OID) {
            // Lấy số cuối cùng từ TenO (sau dấu chấm cuối)
            const parts = item.TenO.split('.');
            const oNumber = parseInt(parts[parts.length - 1]) || 0;

            tangGroups[item.TangID].os.push({
                oId: item.OID,
                oName: item.TenO,
                oNumber: oNumber,
                dai: item.Dai,
                rong: item.Rong,
                cao: item.Cao,
                textO: item.TextO || ''
            });
        }
    });

    // Convert to array và sort (tầng cao xuống tầng thấp)
    const tangList = Object.values(tangGroups).sort((a, b) => b.tangNumber - a.tangNumber);

    // Tìm số cột lớn nhất
    const maxCols = Math.max(...tangList.map(t => t.os.length), 1);

    // Tạo HTML table
    let html = `
        <div class="table-responsive" style="max-height: 500px; overflow: auto;">
            <table class="table table-bordered" style="font-size: 0.75rem; margin-bottom: 0;">
                <tbody>
    `;

    // Render từng tầng
    tangList.forEach((tang, index) => {
        // Sort các ô theo ONumber (từ trái sang phải)
        tang.os.sort((a, b) => a.oNumber - b.oNumber);

        html += `<tr class="${tang.tangId}" data-idtang="${tang.tangId}" data-texttang="${tang.textTang}">`;

        // Cột tên tầng
        html += `
            <td class="text-center align-middle" 
                style="background: linear-gradient(45deg, #667eea, #764ba2); 
                       color: white; 
                       font-weight: bold; 
                       width: 100px;
                       padding: 8px;">
                 <div style="min-width: 55px;">${tang.tangName}</div> </td>
            <td class="text-center align-middle"
                style="background: #Fff;
                        color: white;
                        min-width: 55px;
                        font-weight: bold;
                        width: 100px;
                        padding: 1px;
                        position: relative"> <div style="max-width:100px">  <span style="word-wrap: break-word;"class="newTextTang text-black">${tang.textTang}</span>
                          <i style="cursor: pointer;
                            color: #b58080;
                            position: absolute;
                            top: 1px;
                            right: 6px;
                            font-size: 14px;" class="fa-solid fa-pen-to-square editTangO "></i>
            </div></td>
        `;

        // Render các ô
        tang.os.forEach((o, index) => {
            const isLastCell = (index === tang.os.length - 1);
            const colspan = isLastCell ? (maxCols - tang.os.length + 1) : 1;

            html += `
                <td class="${o.oId}" data-ido="${o.oId}" class="text-center vitrikho-cell"
                    colspan="${colspan}" 
                    style="padding: 8px; 
                           min-width: 110px;
                           cursor: pointer;
                           transition: all 0.2s;
                           border: 2px solid #e0e0e0;
                           background: #f8f9ff;
                            text-align: center;
                        position: relative";
                    data-o-id="${o.oId}"
                    data-o-name="${o.oName}">
                    <div style="font-weight: bold; 
                                color: #667eea; 
                                margin-bottom: 4px; 
                                font-size: 0.85rem;">
                        ${o.oName}
                    </div>
                    <div class="" style="font-size: 0.75rem; color: #333; font-weight: 600;"><span class="newTextO">${o.textO}</span> <i style="cursor: pointer;
                            color: #b58080;
                            position: absolute;
                            top: 1px;
                            right: 6px;
                            font-size: 14px;" class="fa-solid fa-pen-to-square editOText"></i>
                    </div>
                    <div style="font-size: 0.7rem; 
                                color: #666; 
                                margin-top: 2px;">
                        ${o.dai} × ${o.rong} × ${o.cao} 
                    </div>
                </td>
            `;
        });

        html += `</tr>`;
    });

    html += `
                </tbody>
            </table>
        </div>
    `;

    $('#tbodyTang').html(html);
}

// Hàm render chỉ có tầng (chưa có ô) - DẠNG TABLE
function renderTangListOnly(tangs, dayId, keId) {
    if (!tangs || tangs.length === 0) {
        $('#tbodyTang').html('<div class="alert alert-warning text-center">Kệ này chưa có tầng nào</div>');
        return;
    }

    // Sort tầng cao xuống tầng thấp
    const sortedTangs = [...tangs].sort((a, b) => {
        const numA = parseInt(a.name.replace('Tầng', '').replace('tầng', '').trim());
        const numB = parseInt(b.name.replace('Tầng', '').replace('tầng', '').trim());
        return numB - numA;
    });

    let html = `
        <div class="alert alert-info mb-3" style="font-size: 0.85rem;">
            <i class="fas fa-info-circle me-2"></i>
            <strong>Thông tin:</strong> Kệ này có ${sortedTangs.length} tầng nhưng chưa có ô nào. Vui lòng tạo ô cho các tầng.
        </div>
        <div class="table-responsive" style="max-height: 500px; overflow: auto;">
            <table class="table table-bordered" style="font-size: 0.75rem; margin-bottom: 0;">
                <tbody>
    `;

    sortedTangs.forEach(tang => {
        html += `
            <tr class="${tang.id}" data-idtang="${tang.id}" data-textTang="${tang.textTang}">
                <td class="text-center align-middle" 
                    style="background: linear-gradient(45deg, #667eea, #764ba2); 
                           color: white; 
                           font-weight: bold; 
                           width: 100px;
                           padding: 8px;">
                    <div style="min-width: 55px;">${tang.name}</div> </td>
                 <td class="text-center align-middle"
                style="background: #Fff;
                        color: white;
                        font-weight: bold;
                        width: 100px;
                        padding: 8px;
                        position: relative;
                        "> <div style="max-width:100px">  <span style="word-wrap: break-word;" class="newTextTang text-black">${tang.textTang}</span>
                            <i style="cursor: pointer;
                            color: #b58080;
                            position: absolute;
                            top: 1px;
                            right: 6px;
                            font-size: 14px;" class="fa-solid fa-pen-to-square editTangO"></i>
                 </div></td>
                <td class="text-center align-middle tang-empty-cell" 
                    style="padding: 5px;
                           background: #f8f9ff;
                           border: 2px dashed #ccc;
                           cursor: pointer;
                           transition: all 0.3s;"
                    data-tang-id="${tang.id}"
                    data-tang-name="${tang.name}">
                    <div style="display: flex; 
                                align-items: center; 
                                justify-content: center; 
                                gap: 15px;">
                        <i class="fas fa-inbox" style="font-size: 2rem; color: #ccc;"></i>
                        <div>
                            <div style="color: #999; font-size: 0.85rem; margin-bottom: 8px;">
                                Chưa có ô nào
                            </div>
                            <button class="btn btn-sm btn-primary btn-create-o-for-tang d-none" 
                                    data-tang-id="${tang.id}"
                                    data-tang-name="${tang.name}"
                                    onclick="event.stopPropagation();">
                                <i class="fas fa-plus me-1"></i>Tạo Ô Cho Tầng Này
                            </button>
                        </div>
                    </div>
                </td>
            </tr>
        `;
    });

    html += `
                </tbody>
            </table>
        </div>
    `;

    $('#tbodyTang').html(html);


}

// Thêm event click cho các ô
$('#tbodyTang').on('click', '.vitrikho-cell', function () {
    const oId = $(this).data('o-id');
    const oName = $(this).data('o-name');

    // Bỏ highlight cũ
    $('.vitrikho-cell').removeClass('selected-cell');

    // Thêm highlight mới
    $(this).addClass('selected-cell');

    // Gọi function xử lý khi click ô (nếu có)
    if (typeof onOCellClick === 'function') {
        onOCellClick(oId, oName);
    }

    console.log('Đã chọn ô:', oName, oId);
});
// Callback function khi click vào ô (có thể customize)
function onOCellClick(oId, oName) {
    // Xử lý khi click vào ô
    // VD: Hiện modal, load thông tin chi tiết, etc.
    console.log('Ô được chọn:', oId, oName);
}
$(document).ready(function () {
    // Thêm event click cho các ô
    $('#tbodyTang').on('click', '.vitrikho-cell', function () {
        const oId = $(this).data('o-id');
        const oName = $(this).data('o-name');

        // Bỏ highlight cũ
        $('.vitrikho-cell').removeClass('selected-cell');

        // Thêm highlight mới
        $(this).addClass('selected-cell');

        console.log('Đã chọn ô:', oName, oId);
    });
    $('#tbodyTang').on('click', '.editTangO', function () {
        trEditTang = $(this).closest("tr").attr("class")
        const tr = $(this).closest("tr")
        const tangid = tr.data("idtang")
        const textTang = tr.find(".newTextTang").text()
        EditNameTang(tangid, textTang)
    });

    $('#tbodyTang').on('click', '.editOText', function () {
        trEditO = $(this).closest("td").attr("class")
        const td = $(this).closest("td")
        const texto = td.find(".newTextO").text()
        EditNameO(trEditO, texto)
    });
    $('#previewOTableBody').on('click', '.editOText', function () {
        trEditOCreate = $(this).closest("div").attr("class")
        const td = $(this).closest("div")
        const texto = td.find(".newTextO").text()
        EditNameOCreate(texto)
    });
    // Thêm event click cho nút "Tạo Ô"
    $('#tbodyTang').on('click', '.btn-create-o-for-tang', function (e) {
        e.stopPropagation();
        const tangId = $(this).data('tang-id');
        const tangName = $(this).data('tang-name');

        console.log('Tạo ô cho tầng:', tangName, tangId);

        // Mở modal tạo ô
        $('#openCreateOModal').click();
        showToast('info', `Vui lòng tạo ô cho ${tangName}`);
    });
    $("#btnDeleteAllTangO").on("click",async function () {
        showConfirmDeleteModal("tất cả các tầng và ô", async function () {
            let arrDelete = []
            $("#tbodyTang tr").each(function () {
                let object = {
                    TangID: $(this).attr("class")

                }
                arrDelete.push(object)
            })
            try {
                const checkDelete = await Delete(arrDelete, 3, 1)
                if (checkDelete) {
                    showToast("success", "Xóa thành công!")
                    await GetWarehouseData(1)
                    await renderTang([], keLocal, dayLocal)
                } else {
                    showToast("error", "Xóa thất bại")
                }
            } catch (error) {
                console.log(error)
            }
        });
        
    })
});