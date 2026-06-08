let user = localStorage.getItem("username1")
$(".selectDs").select2()
function renderDonHang() {
    $.ajax({
        url: '/api/ThongKeDongThung/Get?Action=GetDonHang&Para1=' + user + '&Para2=A&Para3=A&Para4=A&Para5=A',
        type: "Get",
        success: function (data) {
            $(".donhang").empty()
            emptyTable()
            if (data.length == 0) return;
            $.each(data, (index, item) => {
                $(".donhang").append(`
                 <option data-tenhang="${item.TenHang}" value="${item.MaDH}">${item.MaHangDisplay}</option>

                `)
            })
            renderPO()
        }
    })
}
function renderPO() {
    $.ajax({
        url: '/api/ThongKeDongThung/Get?Action=GetPO&Para1=' + $(".donhang").val() + '&Para2=a&Para3=A&Para4=A&Para5=A',
        type: "Get",
        success: function (data) {
            emptyTable()
            $(".po").empty();
            if (data.length == 0) return;
            $.each(data, (index, item) => {
                $(".po").append(`
                   <option value="${item.POID}">${item.PO}</option>
                `)
            })
            renderMKL()
        }
    })
}
function renderMKL() {
    $.ajax({
        url: '/api/ThongKeDongThung/Get?Action=GetMKL&Para1=' + $(".donhang").val() + '&Para2=' + $(".po").val() + '&Para3=a&Para4=A&Para5=A',
        type: "Get",
        success: function (data) {
            $(".mapkl").empty();
            if (data.length == 0) return;
            $.each(data, (index, item) => {
                $(".mapkl").append(`
                   <option value="${item.MaPKL}">${item.Display}</option>
                `)
            })
            renderTable()
        }
    })
}
let arrChiTiet = [];
let arrTong = [];
function renderTable() {
    $.ajax({
        url: '/api/ThongKeDongThung/GetCTDH?Action=GetChiTietDT&Para1=' + $(".donhang").val() + '&Para2=' + $(".po").val() + '&Para3=' + $(".mapkl").val() + '&Para4=a&Para5=A',
        type: "Get",
        success: function (data) {
            emptyTable()
            if (data.ChiTiet.length == 0) return; // Kiểm tra xem dữ liệu trả về có hợp lệ không
            arrChiTiet = data.ChiTiet;
            arrTong = data.Total;
            let keys = Object.keys(arrChiTiet[0]);
            let filteredKeys = keys.filter(key => key.includes('@SIZE_'));

            let uniqueValues = arrChiTiet.map(x => ({
                MaPKL: x.MaPKLDisplay,
                SttThung: x.SttThung,
                SLThung: x.SLThung,
                SoLuong: x.SoLuong,
                TotalPiece: x.TotalPiece,
                TrongLuong: x.TrongLuong,
                KhoiLuong: x.KhoiLuong,

            }))
            uniqueValues = uniqueValues.filter((value, index, self) =>
                index === self.findIndex(obj =>
                    obj.MaPKL === value.MaPKL &&
                    obj.SttThung === value.SttThung
                )
            );
            let totalSLThung = uniqueValues.reduce((acc, curr) => acc + parseInt(curr.SLThung), 0);
            let totalTotalPiece = uniqueValues.reduce((acc, curr) => acc + parseInt(curr.TotalPiece), 0);
            let totalSoLuong = uniqueValues.reduce((acc, curr) => acc + parseInt(curr.SoLuong), 0);
            var totalTrongLuong = uniqueValues.reduce((acc, curr) => acc + parseFloat(curr.TrongLuong), 0);
            var roundedTotalTrongLuong = Math.round(totalTrongLuong * 10) / 10;
            var totalKhoiLuong = uniqueValues.reduce((acc, curr) => acc + parseFloat(curr.KhoiLuong), 0);
            var roundedTotalKhoiLuong = Math.round(totalKhoiLuong * 10) / 10;
            let $trSize = "";
            $.each(filteredKeys, (index, item) => {
                item = item.split("@")[0]
                $trSize += `
                            <td> ${item} </td>
                            `
            })
            $("#thead").append(`
                     <tr  style="position:relative">
                                     <td style="position: sticky; top: 0;display:none" rowspan="2" >SttThung</td>
                                    <td style="position: sticky; top: 0;" rowspan="2" colspan="2">Carton Number</td>
                                    <td style="position: sticky; top: 0;" rowspan="2">PO</td>
                                    <td style="position: sticky; top: 0;" rowspan="2">Đơn vị sản xuất</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Đợt SX</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">InSeam</td>
                                    <td style=" position: sticky; top: 0;"rowspan="2">Color</td>
                                    <td style=" position: sticky; top: 0;" colspan="${filteredKeys.length}">Size</td>
                                    <td style=" position: sticky; top: 0;"rowspan="2">Qty </td>
                                    <td style="position: sticky; top: 0;" rowspan="2">Carton </td>
                                    <td style="position: sticky; top: 0;" rowspan="2">Total Piece</td>
                                    <td style="position: sticky; top: 0;" rowspan="2">NW</td>
                                    <td style="position: sticky; top: 0;" rowspan="2">GW</td>
                                    <td style="position: sticky; top: 0;" rowspan="2">Carton</td>
                                </tr>
                                 <tr >
                                   ${$trSize}
                                </tr>   `)
            $.each(arrChiTiet, (index, item) => {
                let html = ``;
                $.each(filteredKeys, (index, lstItem) => {
                    let value = item[lstItem];
                    if (value === 0) {
                        html += `<td></td>`;
                    } else {
                        html += `<td>${value}</td>`;
                    }
                })
                $("#tbody").append(`
                        <tr>
                            <td class="rowMerge" style="display:none">${item.SttThung}</td>
                            <td class="rowMerge"> ${item.TuThung}</td>
                            <td class="rowMerge">${item.DenThung}</td>
                            <td style="white-space: nowrap;">${item.PO}</td>
                            <td  style="white-space: nowrap;">${item.TenDVSX}</td>
                            <td>${item.DotSX}</td>
                            <td style="white-space: nowrap;">${item.DauSize}</td>
                            <td style="white-space: nowrap;">${item.TenMau}</td>
                            ${html}
                            <td class="rowMerge" >${item.SoLuong}</td>
                            <td  class="rowMerge">${item.SLThung}</td>
                            <td  class="rowMerge">${item.TotalPiece}</td>
                            <td class="rowMerge">${(item.TrongLuong).toFixed(2)}</td>
                            <td class="rowMerge">${(item.KhoiLuong).toFixed(2)}</td>
                            <td>${item.KyHieuA}</td>
                        </tr>
    
                    `)

            })
            $("#tfoot").append(`
                    <tr>
                        <td colspan="${filteredKeys.length + 7}"></td>
                        <td>${totalSoLuong}</td>
                        <td>${totalSLThung}</td>
                        <td>${totalTotalPiece}</td>
                        <td>${roundedTotalTrongLuong}</td>
                        <td>${roundedTotalKhoiLuong}</td>
                         <td ></td>
                    </tr>
                `)

            $("#theadTotal").append(`
                     <tr  style="position:relative">
                                    <td style="position: sticky; top: 0;" rowspan="2" >InSeam</td>
                                    <td style="position: sticky; top: 0;" rowspan="2">Tên màu</td>
                                    <td style=" position: sticky; top: 0;" colspan="${filteredKeys.length}">Size</td>
                                    <td style=" position: sticky; top: 0;"rowspan="2">Tổng</td>
                                </tr>
                                 <tr >
                                   ${$trSize}
                                </tr> `)
            let TongSize = 0;
            $.each(arrTong, (index, item) => {
                let htmlTotal = ``;
                TongSize += item.TongSize;
                $.each(filteredKeys, (index, lstItem) => {
                    itemToal = lstItem.split("@")[0]
                    let value = item[lstItem];
                    if (value === 0) {
                        htmlTotal += `<td class="${itemToal}"></td>`;
                    } else {
                        htmlTotal += `<td class="${itemToal}">${value}</td>`;
                    }
                })
                $("#tbodyTotal").append(`
                        <tr>
                            <td style="white-space: nowrap;">${item.DauSize}</td>
                            <td style="white-space: nowrap;">${item.TenMau}</td>
                            ${htmlTotal}
                            <td>${item.TongSize}</td>
                        </tr>   
    
                    `)

            })
            let arrSumSize = [];
            $.each(filteredKeys, (index, item) => {
                itemToal = item.split("@")[0]
                let total = 0;
                $(`#tbodyTotal .${itemToal}`).each(function () {
                    let value = parseFloat($(this).text());
                    if (!isNaN(value)) {
                        total += value;
                    }
                });
                arrSumSize.push(total);
            });
            let htmlTfoot = "";
            $.each(arrSumSize, (index, item) => {
                htmlTfoot += `
                    <td> ${item}</td>
                `
            })
            $("#tfootTotal").append(`
                    <tr>
                        <td colspan="2"></td>
                         ${htmlTfoot}
                        <td>${TongSize}</td>
                    </tr>
                `)
            MergeRow()
        }
    })
}
function emptyTable() {
    $("#tbody").empty();
    $("#thead").empty();
    $("#tfoot").empty();
    $("#tbodyTotal").empty();
    $("#theadTotal").empty();
    $("#tfootTotal").empty();
}
//change 
$(".donhang").on("change", function () {
    renderPO()
})
$(".po").on("change", function () {
    renderMKL()
})
$(".mapkl").on("change", function () {
    renderTable()
})
$("#detail").on("click", function () {
    $("#total").removeClass("active2")
    $("#detail").removeClass("active2")
    $(this).addClass("active2")
    $(".lstChiTiet").show()
    $(".lstTotal").hide()
})
$("#total").on("click", function () {
    $("#total").removeClass("active2")
    $("#detail").removeClass("active2")
    $(this).addClass("active2")
    $(".lstChiTiet").hide()
    $(".lstTotal").show()
})
function MergeRow() {
    let tableLength = $("#tbody tr").length;

    for (let i = 0; i < tableLength; i++) {
        let merge = 1;
        let valueMerge = $("#tbody tr").eq(i).find(".rowMerge").text();
        for (let j = i + 1; j < tableLength; j++) {
            let valueMergeNext = $("#tbody tr").eq(j).find(".rowMerge").text();
            if (valueMerge === valueMergeNext) {
                merge++;
            } else {
                break;
            }
        }
        $("#tbody tr").eq(i).find(".rowMerge").attr("rowspan", merge);
        for (let k = 1; k < merge; k++) {
            $("#tbody tr").eq(i + k).find(".rowMerge").hide();
        }
        i += merge - 1;
    }
}
$("#Excel").on("click", function () {
    Export()
})
function Export() {
    var url = '/api/ThongKeDongThung/GetExcel?Action=GetChiTietDT&Para1=' + $(".donhang").val() + '&Para2=' + $(".po").val() + '&Para3=' + $(".mapkl").val() + '&Para4='
        + user + '&Para5=A&Para6=' + $(".donhang option:selected").data("tenhang")

    var link = document.createElement('a');
    var filename = `ThongKeDonHang.xlsx`;
    link.href = url;
    link.download = filename;
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
$("#home").on("click", function () {
    window.location.href = '/Home/Dashboard'
})
renderDonHang()