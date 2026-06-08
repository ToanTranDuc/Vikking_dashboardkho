
toastr.options = {
    "closeButton": true,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "3000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}
$(document).ready(function () {
    if (window.CefSharp) {
        $(".fa-house").hide();
    }
});
$(function () {
    renderNhaMay()
    $(".nhamay").on("change", function () {
        renderTenKho()
        $(".rendertable_list1").empty();
    })
    $(".tenkho").on("change", function () {
        renderTenKhuVuc()
        getDHSearch()
        $(".rendertable_list1").empty();
    })
    $(".tenkhuvuc").on("change", function () {
        renderKe(1)
        renderKeNew()
        $(".rendertable_list1").empty();
        $(".search_vitri").val("")
    })

})
function removeLoading() {
    $(".list_loading").addClass("active")
}
function addLoading(value) {
    if(value == 1) console.log("tognquan")
    $(".list_loading").removeClass("active")
}
let dataDvSx = data.dvsx
let dataTenKho = data.tenkho
let dataTenKhuVuc = data.tenkhuvuc
let userNameWin = data.username
let checkMaHang = "1"
$(".checkViTri").on("change", function () {
    $(".tbody td").removeClass("active");
    $(".tbody td").removeClass("activeHide");
    if ($(this).prop("checked")) {
        checkMaHang = 2
        searchMaHang($(".search_vitri").val())
    }
    else {
        checkMaHang = 1
        searchMaHang($(".search_vitri").val())
    }
})
$(".search_vitri").on("input", function () {
    let searchValue = $(this).val().toLowerCase();
    $(".tbody td").removeClass("active");
    $(".tbody td").removeClass("activeHide");
    $(".tbody td").find(".gopitem").removeClass("activeTD");
    searchMaHang(searchValue);
});

function searchMaHang(searchValue) {
    $(".rendertable_list table").find("td").each(function () {
        let $this = $(this);
        if ($this.hasClass("rowMerge") || $this.hasClass("trTfoot")) return;

        $this.show().removeClass("active");
        if ($this.find('.gopitem').length > 0) {
            $this.find('.gopitem').each(function () {
                let $gopitem = $(this);
                let mahang = $gopitem.data("mahang").toString();
                let po = $gopitem.data("po").toString();
                if (searchValue != "" && mahang && mahang.toLowerCase().includes(searchValue.toLowerCase()) || searchValue != "" && po && po.toLowerCase().includes(searchValue.toLowerCase())) {
                    $this.addClass("active");
                    $gopitem.addClass("activeTD")
                }
            });
        }
    });

    if (checkMaHang != 1) {
        $(".rendertable_list table").find("td").each(function () {
            let $this = $(this);
            if ($this.hasClass("rowMerge") || $this.hasClass("trTfoot")) return;
            if (!$this.hasClass("active")) {
                $this.hide();
            }
        });
    }
}

$(".btnChiTiet").on("click", function () {
    const dvsx = $(".nhamay").val()
    const tenkho = $(".tenkho").val()
    const tenkhuvuc = $(".tenkhuvuc").val()
    const indexViTri = "0"
    window.location.href = `/SoDoKho/SoDo?dvsx=${dvsx}&tenkho=${tenkho}&tenkhuvuc=${tenkhuvuc}&indexViTri=${indexViTri}&username=${user}`;
})
$(".btnChiTietGiaCong").on("click", function () {
    const dvsx = $(".nhamay").val()
    const tenkho = $(".tenkho").val()
    const tenkhuvuc = $(".tenkhuvuc").val()
    const indexViTri = "0"
    window.location.href = `/SoDoKho/SoDoGiaCong?dvsx=${dvsx}&tenkho=${tenkho}&tenkhuvuc=${tenkhuvuc}&indexViTri=${indexViTri}&username=${user}`;
})
var user = userNameWin == null ? localStorage.getItem("username1") : userNameWin
$(".tenkho").select2()
$(".tenkhuvuc").select2()

function renderNhaMay() {
    $.ajax({
        url: `/api/ViTriKhoNew/Get?Action=GetNhaMayKho&Para1=${user}&Para2=a&Para3=a&Para4=a&Para5=a&Para6=a`,
        type: 'Get',
        success: function (data) {
            let html = ``
            data.map(item => {
                html += `
                        <option  value="${item.MaDVSX}">${item.TenDVSX}</option>
                `
            })
            $(".nhamay").html(html)
            $(".nhamay").select2()
            if (dataDvSx != null) {
                $(".nhamay").val(dataDvSx).trigger("change")
            }
            else {
                renderTenKho()
            }
        }
    })
}
function renderTenKho() {
    $.ajax({
        url: `/api/ViTriKhoNew/Get?Action=GetViTriKho&Para1=${$(".nhamay").val()}&Para2=a&Para3=a&Para4=a&Para5=a&Para6=a`,
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                $(".tenkho").empty()
                $(".tenkhuvuc").empty()
                $(".rendertable_list").empty();
                return;
            }
            let html = ``
            data.map(item => {
                html += `
                        <option  value="${item.MaKho}">${item.TenKho}</option>
                `
            })
            $(".tenkho").html(html)
            $(".tenkho").select2()
            if (dataDvSx != null) {
                $(".tenkho").val(dataTenKho).trigger("change")
            }
            else {
                renderTenKhuVuc()
            }
            getDHSearch()
        }
    })
}
let datakhuvuc = []
function renderTenKhuVuc() {
    $.ajax({
        url: `/api/ViTriKhoNew/Get?Action=GetDay&Para1=${$(".nhamay").val()}&Para2=${$(".tenkho").val()}&Para3=a&Para4=a&Para5=a&Para6=a`,
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                $(".tenkhuvuc").empty()
                $(".rendertable_list").empty();
                return;
            }
            datakhuvuc = data
            let html = ``
            data.map(item => {
                html += `
                        <option  value="${item.MaDay}">${item.NameDay}</option>
                `
            })
            $(".tenkhuvuc").html(html)
            $(".tenkhuvuc").select2()
            if (dataDvSx != null) {
                $(".tenkhuvuc").val(dataTenKhuVuc).trigger("change")
            }
            else {
                renderKe(1)
                renderKeNew()
            }

        }
    })
}
async function renderKe(value) {
    try {
        addLoading()
        const data = await $.ajax({
            url: `/api/ViTriKhoNew/Get?Action=GetKe&Para1=${$(".nhamay").val()}&Para2=${$(".tenkho").val()}&Para3=${$(".tenkhuvuc").val()}&Para4=a&Para5=a&Para6=a`,
            type: 'GET'
        });
        if (data.length == 0) {
            removeLoading()
            $(".rendertable_list").empty();
        }
        let htmlTangFomat = ""
        let htmlTotal = "";
        for (const item of data) {
            let make = item.MaKe;
            let tenke = item.NameKe;
            let htmlKe = "";

            const dataTang = await renderTang(make);
            if (dataTang.length == 0) continue;
            let htmlTang = ""
            let indexT = 1;
            for (const tang of dataTang) {
                let matang = tang.MaTang;
                let tenTang = tang.NameTang;
                const dataO = await renderO(make, matang);  

                let htmlO = ""
                for (const o of dataO) {
                    let phantram = ((o.CBM - o.CBMTotal) / o.CBM) * 100;
                    let iconmau = "";
                    let cbmO = Math.round(o.CBMTotal * 1000) / 1000
                    let valueo = `<p class="nameo" style="margin-top:2px;white-space: nowrap;margin-left: 5px;">${o.NameO}</p>`
                    let chuyenIndex = `<i class="fa-solid fa-arrow-right arrowIndex"></i>`
                    if (100 - phantram <= 0) {
                        iconmau = `
                            ${valueo} ${chuyenIndex} <div class="list_p"> <i style="  font-size: 12px; padding: 0 2px; color:" class="fa-regular fa-circle"></i><p class="CBMTD">(${cbmO}/${o.CBM})</p></div>`;
                    } else if (100 - phantram > 80) {
                        iconmau = `
                            ${valueo} ${chuyenIndex} <div class="list_p"> <i style=" font-size: 12px; padding: 0 2px; color:#00FF00" class="fa-solid fa-circle"></i><p class="CBMTD">(${cbmO}/${o.CBM})</p></div>`;
                    } else if (100 - phantram >= 50 && 100 - phantram <= 80) {
                        iconmau = ` 
                                    ${valueo} ${chuyenIndex} <div class="list_p"><i style=" font-size: 12px; padding: 0 2px;color:#F6B26B" class="fa-solid fa-circle"></i><p class="CBMTD">(${cbmO}/${o.CBM})</p> </div>`;
                    } else if (100 - phantram < 50) {
                        iconmau = ` 
                                    ${valueo} ${chuyenIndex} <div class="list_p"><i style=" font-size: 12px; padding: 0 2px;color: #FF0000" class="fa-solid fa-circle"></i><p class="CBMTD">(${cbmO}/${o.CBM})</p></div> `;
                    }
                    let htmlnew = ""
                    //if (cbmO != o.CBM)
                         //htmlnew = await renderThungInKe(make, matang, o.MaO)
                    htmlO += `<td style="text-align: left;vertical-align: top; "data-cbmcl="${cbmO}" data-make="${make}" data-o="${o.MaO}" data-khoiluong="${o.CBM}" data-tang="${matang}">
                       <div class="list_item">
                           <div class="item_Icon">
                              ${iconmau}
                           </div>
                       <div style="min-width: 200px;">

                           <div class="item_thung"  >
                              ${htmlnew}
                            </div>
                            </div>
                            </div>
                        </div>
                     </td>`;

                }
                indexT++;
                htmlKe = `<td class="rowMerge" style="white-space: ; max-width: 16px; min-width: 30px;background: #a8f6ff;">
                ${tenke}</td>`
                htmlTang += `
                    <tr class>
                    ${htmlKe}
                    ${htmlO}</tr>
                `;
            }
            htmlTangFomat = fomatRenderTang(htmlTang)
            htmlTotal +=
                `
                    <table class="tbody">
                        ${htmlTangFomat}
                       </table>
                `
                ;
            let maxTdCount = findMaxTdCount();
            let htmlFolter = `
               <table style="margin-top:-18px">
                <tr >
                    <td class="trTfoot" colspan=${maxTdCount} style="font-size: 25px;position: sticky;  bottom: 0; background: #fff; color: #016b69; font-family: 'Roboto';text-align: center;" class="myTableCell">Chú thích : Sức chứa còn lại --- (<i style="font-size: 22px;" class="fa-regular fa-circle"></i>)
                      = 0% --- (<i style="font-size: 22px; color:#00FF00" class="fa-solid fa-circle"></i>) > 80% 
                        --- (<i style="font-size: 22px;  color:#FF0000 " class="fa-solid fa-circle"></i>) < 50%
                --- (<i style="font-size: 22px;color:#F6B26B" class="fa-solid fa-circle"></i>) 50% - 80% </td>
                </tr>
                 </table>

            `;

            $(".rendertable_list").html(htmlTotal)
            $(".rendertable_list").append(htmlFolter)
            MergeRow()
            renderListThung()
        }
        removeLoading()
        if (!checkgoiychange) {
            goiyino()
        }
    } catch (error) {
        console.error(error);
    }
}
async function renderListThung() {
    $(".rendertable_list td").each(async function () {
        let $thisTD = $(this)
        let make = $thisTD.data("make")
        let matang = $thisTD.data("tang")
        let mao = $thisTD.data("o")
        let cbmcl = $thisTD.data("cbmcl")
        let cbm = $thisTD.data("khoiluong")
        if (cbm == cbmcl) return;
        let htmlnew2 = await renderThungInKe(make, matang, mao)
        $thisTD.find(".item_thung").html(htmlnew2)
    })
}
function findMaxTdCountFromHtml(html) {
    let maxTdCount = 0;
    let container = $(html);
    container.find('.tbody').each(function () {
        let table = $(this);
        table.find('tr').each(function () {
            let rowCount = $(this).find('td').length; // Đếm số lượng cột của từng hàng trong tbody
            if (rowCount > maxTdCount) {
                maxTdCount = rowCount; // Cập nhật giá trị maxTdCount nếu có số cột lớn hơn
            }
        });
    });
    return maxTdCount;
}
function fomatRenderTang(htmlTangFomat) {
    const wrappedHtml = `<table>${htmlTangFomat}</table>`;
    const rows = $(wrappedHtml).find('tr');

    let maxCols = 0;

    // Find the maximum number of columns
    rows.each(function () {
        const cols = $(this).find('td').length;
        if (cols > maxCols) {
            maxCols = cols;
        }
    });

    // Adjust each row to have the same number of columns
    let htmltr = '';
    rows.each(function () {
        const cols = $(this).find('td').length;
        if (cols < maxCols) {
            const lastTd = $(this).find('td').last();
            const colspan = maxCols - cols + 1;
            lastTd.attr('colspan', colspan);
        }
        htmltr += `<tr>${$(this).html()}</tr>\n`;
    });

    return htmltr
}



function findMaxTdCount() {
    let maxTdCount = 0;

    $(".tbody").find("tr").each(function () {
        let tdCount = $(this).find("td").length;

        if (tdCount > maxTdCount) {
            maxTdCount = tdCount;
        }
    });

    return maxTdCount;
}
function renderTang(make) {
    return $.ajax({
        url: `/api/ViTriKhoNew/Get?Action=GetTang&Para1=${$(".nhamay").val()}&Para2=${$(".tenkho").val()}&Para3=${$(".tenkhuvuc").val()}&Para4=${make}&Para5=a&Para6=a`,
        type: 'GET'
    });
}

function renderO(make, matang) {
    return $.ajax({
        url: `/api/ViTriKhoNew/Get?Action=GetOCBM&Para1=${$(".nhamay").val()}&Para2=${$(".tenkho").val()}&Para3=${$(".tenkhuvuc").val()}&Para4=${make}&Para5=${matang}&Para6=a`,
        type: 'GET'
    });
}
async function renderThungInKe(make, matang, mao) {
    const response = await fetch(`/api/ViTriKhoTong/Get?Action=GetDHKho&Para1=${$(".nhamay").val()}&Para2=${$(".tenkho").val()}&Para3=${$(".tenkhuvuc").val()}&Para4=${make}&Para5=${matang}&Para6=${mao}&Para7=a`);
    const data = await response.json();
    let html = '';
    data.forEach(item => {
        html +=
            `
                 <div data-mahang="${item.MaHang}" data-po="${item.Po}"  data-madh="${item.MaDH}" "
                class="gopitem">
                    <div class="listSearch">
                            <p> <i class="fa-solid fa-circle icon" style="color: #91d219;"></i>MH: ${item.MaHang}</p>
                             <p ><i class="fa-solid fa-circle icon" style="color: #869d5b;"></i>PO: ${item.Po}</p>
                             <p> <i class="fa-solid fa-circle icon" style="color: #19c8d2;"></i>SK: ${item.SLThung}</p>
                            <p> <i class="fa-solid fa-circle icon" style="color: #6219d2;"></i>Size: ${item.Size}</p>
                    </div>
                 
               
            </div>`
            ;
    });
    return html;
}
function MergeRow() {
    let tableLength = $(".tbody tr").length;

    for (let i = 0; i < tableLength; i++) {
        let merge = 1;
        let valueMerge = $(".tbody tr").eq(i).find(".rowMerge").text();
        for (let j = i + 1; j < tableLength; j++) {
            let valueMergeNext = $(".tbody tr").eq(j).find(".rowMerge").text();
            if (valueMerge === valueMergeNext) {
                merge++;
            } else {
                break;
            }
        }
        $(".tbody tr").eq(i).find(".rowMerge").attr("rowspan", merge);
        for (let k = 1; k < merge; k++) {
            $(".tbody tr").eq(i + k).find(".rowMerge").hide();
        }
        i += merge - 1;
    }
}

function MergeRow1() {
    let tableLength = $(".tbody1 tr").length;

    for (let i = 0; i < tableLength; i++) {
        let merge = 1;
        let valueMerge = $(".tbody1 tr").eq(i).find(".rowMerge").text();
        for (let j = i + 1; j < tableLength; j++) {
            let valueMergeNext = $(".tbody1 tr").eq(j).find(".rowMerge").text();
            if (valueMerge === valueMergeNext) {
                merge++;
            } else {
                break;
            }
        }
        $(".tbody1 tr").eq(i).find(".rowMerge, td:eq(0)").attr("rowspan", merge);
        for (let k = 1; k < merge; k++) {
            $(".tbody1 tr").eq(i + k).find(".rowMerge, td:eq(0)").hide();
        }
        i += merge - 1;
    }
}
function mergeTable(tableId, columnIndex) {
    var firstCell = null;
    var rowspan = 1;

    $('.' + tableId + ' tr').each(function () {
        var currentCell = $(this).find('td').eq(columnIndex);

        if (firstCell == null) {
            firstCell = currentCell; // Lưu ô đầu tiên để so sánh
        } else if (currentCell.text() == firstCell.text()) {
            currentCell.remove(); // Xóa ô hiện tại
            rowspan++; // Tăng rowspan của ô đầu tiên
            firstCell.attr('rowspan', rowspan); // Cập nhật rowspan
        } else {
            firstCell = currentCell; // Gán lại ô đầu tiên
            rowspan = 1; // Đặt lại rowspan
        }
    });
}
$(".rendertable_list").on("click", ".arrowIndex", function () {
    let $this = $(this).closest("td")
    let make = $this.data("make")
    let matang = $this.data("tang")
    let mao = $this.data("o")
    let dvsx = $(".nhamay").val()
    let tenkho = $(".tenkho").val()
    let tenkhuvuc = $(".tenkhuvuc").val()
    let indexViTri = ""

    Swal.fire({
        title: 'Lựa chọn chức năng?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Thùng chuyển đến',
        cancelButtonText: 'Hủy',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        showCloseButton: true,// Thêm nút đóng
    }).then((result) => {
        if (result.isConfirmed) {
            indexViTri = 1
            window.location.href = `/SoDoKho/SoDo?dvsx=${dvsx}&tenkho=${tenkho}&tenkhuvuc=${tenkhuvuc}&tenke=${make}&tentang=${matang}&teno=${mao}&indexvitri=${indexViTri}&username=${user}`;

        }

    });
    //$(".swal2-cancel").on("click", function () {
    //    indexViTri = 2
    //    window.location.href = `/SoDoKho/SoDo?dvsx=${dvsx}&tenkho=${tenkho}&tenkhuvuc=${tenkhuvuc}&tenke=${make}&tentang=${matang}&teno=${mao}&indexvitri=${indexViTri}`;
    //})
})
$(".list_tabIndex").on("click", "button", function () {
    $(".list_tabIndex").find("button").removeClass("active")
    $(this).addClass("active")
    var tab_id = $(this).data('tab');
    $('.tab-content').removeClass('active');
    $(`#${tab_id}`).addClass('active');

})
async function renderKeNew() {
    try {
        let htmlTable = "";
        $(".rendertable_list1").empty()
        addLoading(1)
        for (const khuVuc of datakhuvuc) {
            const data = await $.ajax({
                url: `/api/ViTriKhoNew/Get?Action=GetKe&Para1=${$(".nhamay").val()}&Para2=${$(".tenkho").val()}&Para3=${khuVuc.MaDay}&Para4=a&Para5=a&Para6=a`,
                type: 'GET'
            });

            if (data.length === 0) {
                $(".rendertable_list").empty();
                continue;
            }

            let htmlTangFomat = "";
            let htmlTotal = "";

            for (const item of data) {
                let make = item.MaKe;
                let tenke = item.NameKe;
                let htmlKe = "";
                let htmlTenKhuVuc = `<td class="rowMerge" style="white-space: ; max-width: 16px; min-width: 30px;background: #fff8c4;">
               ${khuVuc.NameDay}</td>`;

                const dataTang = await renderTang1(khuVuc.MaDay, make);
                if (dataTang.length === 0) continue;

                let htmlTang = "";
                let indexT = 1;

                for (const tang of dataTang) {
                    let matang = tang.MaTang;
                    let tenTang = tang.NameTang;
                    const dataO = await renderO1(khuVuc.MaDay, make, matang);

                    let htmlO = "";
                    for (const o of dataO) {
                        let phantram = ((o.CBM - o.CBMTotal) / o.CBM) * 100;
                        let iconmau = "";
                        let cbmO = Math.round(o.CBMTotal * 1000) / 1000;
                        let valueo = `<p style="margin:0;white-space: nowrap;padding: 5px 0;">${o.NameO}</p>`;
                        let chuyenIndex = `<i class="fa-solid fa-arrow-right arrowIndex"></i>`;

                        htmlO += `<td data-make="${make}" data-o="${o.MaO}" data-khoiluong="${o.CBM}" data-tang="${matang}">
                           <div class="">
                              ${valueo}
                            </div>
                         </td>`;
                    }
                    indexT++;

                    htmlKe = `<td class="rowMerge" style="white-space: ; max-width: 16px; min-width: 30px;background: #a8f6ff;">
                    ${tenke}</td>`;
                    htmlTang += `
                    <tr>
                    ${htmlTenKhuVuc}
                    ${htmlKe}
                    ${htmlO}</tr>`;
                }
                htmlTangFomat = fomatRenderTang(htmlTang);
                htmlTotal += htmlTangFomat;
            }
            htmlTable += `
            <table class="tbody1">
                ${htmlTotal}
            </table>`;
            $(".rendertable_list1").empty()
            $(".rendertable_list1").append(htmlTable)
        }

        MergeRow1();
        mergeTable('tbody1', 0);
        removeLoading()
    } catch (error) {
        console.error(error);
    }
}
function renderTang1(khuvuc, make) {
    return $.ajax({
        url: `/api/ViTriKhoNew/Get?Action=GetTang&Para1=${$(".nhamay").val()}&Para2=${$(".tenkho").val()}&Para3=${khuvuc}&Para4=${make}&Para5=a&Para6=a`,
        type: 'GET'
    });
}

function renderO1(khuvuc, make, matang) {
    return $.ajax({
        url: `/api/ViTriKhoNew/Get?Action=GetOCBM&Para1=${$(".nhamay").val()}&Para2=${$(".tenkho").val()}&Para3=${khuvuc}&Para4=${make}&Para5=${matang}&Para6=a`,
        type: 'GET'
    });
}
$(".madh,.PO,.maPKL").select2()
async function getDHSearch() {
    const dvsx = $(".nhamay").val()
    const tenkho = $(".tenkho").val()
    
    const [response1] = await Promise.all([
        fetch(`/api/ViTriKhoTong/Get?Action=GetDHTong&Para1=${dvsx}&Para2=${tenkho}&Para3=a&Para4=a&Para5=a&Para6=a&Para7=a&Para8=a&Para9=a`),
    ]);
    let data = await response1.json()
    let html = ` `
    data.map(item => {
        html += `
                    <option data-mahang="${item.MaHang}" data-magop="${item.MaDH}"  value="${item.MaGop}">${item.MaDH} - ${item.MaHang} </option>
                `
    })
    $(".madh").html(html)
    renderPO()
}
$(".madh").on("change", function () {
    renderPO()
})
let dataAll = [];
function renderPO() {
    let makho = $(".tenkho").val()
    let maDH = $(".madh").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetAllPO&Para1=${maDH}&Para2=A&Para3=a&Para4=a&Para5=a&Para6=a&Para7=a`,
        type: "Get",
        success: function (data) {
            if (data.length == 0) {
                $(".maPKL").empty()
                return
            }
            dataAll = data
            let dataPO = dataAll.map(item => {
                return { PO: item.PO, POID: item.POID };
            });
            let unniqueMap = dataPO.reduce((unique, item) => {
                return unique.some(t => t.POID === item.POID) ? unique : [...unique, item];
            }, []);
            let html = ` `
            unniqueMap.map(item => {
                html += `
                    <option  value="${item.POID}">${item.PO}</option>
                `
            })
            $(".PO").html(html)   
            renderPKL()
        }

    })
}
$(".PO").on("change", function () {
    renderPKL()
})
function renderPKL() {
    let dataMPL = dataAll.filter(x => x.POID == $(".PO").val())
    let dataMPLNew = dataMPL.map(item => {
        return { MaPKL: item.MaPKL };
    });
    let unniqueMap = dataMPLNew.reduce((unique, item) => {
        return unique.some(t => t.MaPKL === item.MaPKL) ? unique : [...unique, item];
    }, []);
    let html = ` `
   
    unniqueMap.map(item => {
        html += `
                    <option value="${item.MaPKL}">${item.MaPKL} </option>
                `
    })
    $(".maPKL").html(html)
}
$(".filtervitri").on("click", filterO)
async function filterO() {
    let maDH = $(".madh").val()
    let PO = $(".PO").val()
    let pkl = $(".maPKL").val()
    let dvsx = $(".nhamay").val()
    let tenkho = $(".tenkho").val()
    const [response1] = await Promise.all([
        fetch(`/api/ViTriKhoTong/Get?Action=SearchO&Para1=${maDH}&Para2=${PO}&Para3=${dvsx}&Para4=${tenkho}&Para5=a&Para6=a&Para7=a&Para8=a&Para9=a`),
    ]);
    let data = await response1.json()
    filterViTri(data)
}
let nameo = []
function filterViTri(arrKVNew) {
    nameo = []
    let arrKhuVuc = []
    for (var i = 0; i < arrKVNew.length; i++) {
        nameo.push(arrKVNew[i].NameO)
        let object = {
            MaKhuVuc: arrKVNew[i].MaDay,
            TenKhuVuc: arrKVNew[i].TenDay
        }
        arrKhuVuc.push(object)
    }
   let resultKV = arrKhuVuc.reduce((unique, item) => {
        return unique.some(t => t.MaKhuVuc === item.MaKhuVuc) ? unique : [...unique, item];
    }, []);
    if (resultKV.length > 1 || $(".tenkhuvuc").val() != resultKV[0].MaKhuVuc) {
        $('#myModal3').modal('show');
        let html = ``
        resultKV.map(item => {
            html += `
            <div class="checkkv">
                        <div class="task_css"></div>
                        <button id="${item.MaKhuVuc}" class="item_kv">${item.TenKhuVuc}</button>
            </div>
            `
        })
        $(".modal-body").html(html)
    } else {
        goiyino()
    }
}
let checkgoiychange = true
function goiyino() {
    if ($(".tenkho").val() == undefined) {
        toastr.warning('Vui lòng chọn tên kho', 'Warning');
        return;
    }
    let result = nameo
    $(".rendertable_list").find(".nameo").each(function () {
        let value = $(this).text()
        if (result.includes(value)) {
            $(this).closest("td").show()
        }
        else {
            $(this).closest("td").hide()
        }
    })
    checkgoiychange = true
}
$(".modal-body").on("click", '.item_kv', function () {
    changekv($(this).attr("id"))
})
function changekv(id) {
    let valuekv = $(".tenkhuvuc").val()
    if (valuekv == id) {
        goiyino()
    }
    else {
        checkgoiychange = false
        $(".tenkhuvuc").val(id).trigger("change")
    }
    $('#myModal3').modal('hide');
}
$(".eye-dh").on("click", function () {
    $(this).toggleClass("fa-eye fa-eye-slash")
    if ($(this).hasClass("fa-eye")) {
        $(".list_Ds").removeClass("active")
       
    }
    else {
        $(".list_Ds").addClass("active")
        $(".rendertable_list").find(".nameo").each(function () {
            $(this).closest("td").show()
        })
    }
})