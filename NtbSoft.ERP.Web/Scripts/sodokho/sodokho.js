
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
let flagCheckNew = true;
$(function () {
    renderNhaMay()
    $(".nhamay").on("change", function () {
        renderTenKho()
    })
    $(".tenkho").on("change", function () {
        renderTenKhuVuc()
        rederPageDay()
    })
    $(".tenkhuvuc").on("change", function () {
        renderKe(1)
        //rederPageKe()
    })

})
function removeLoading() {
    $(".list_loading").addClass("active")
}
function addLoading() {
    $(".list_loading").removeClass("active")
}
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
    $(".tbody td").find(".gopitem1").removeClass("activeTD");
    searchMaHang(searchValue);
});

function searchMaHang(searchValue) {
    $(".rendertable_list table").find("td").each(function () {
        let $this = $(this);
        if ($this.hasClass("rowMerge") || $this.hasClass("trTfoot")) return;

        $this.show().removeClass("active");
        if ($this.find('.gopitem1').length > 0) {
            $this.find('.gopitem1').each(function () {
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
var user = data.username
$(".tenkho").select2()
$(".tenkhuvuc").select2()
let dataDvSx = data.dvsx
let dataTenKho = data.tenkho
let dataTenKhuVuc = data.tenkhuvuc
let dataKe = data.tenke
let dataTang = data.tentang
let dataMaO = data.teno
let indexvitri = data.indexvitri
$(".home").on("click", function () {
    const dvsx = $(".nhamay").val()
    const tenkho = $(".tenkho").val()
    const tenkhuvuc = $(".tenkhuvuc").val()
    window.location.href = `/SoDoKho/SoDoTong?dvsx=${dvsx}&tenkho=${tenkho}&tenkhuvuc=${tenkhuvuc}&username=${user}`;
})
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


        }
    })
}
renderDH()
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

            }
            rederPageDay()
            //rederPageKe()
        }
    })
}

async function renderKe(value) {
    try {
        if (value == 1)
            addLoading()
        let checkHide = ($(".eye_all").hasClass("fa-eye")) ? "" : "none"

        const data = await $.ajax({
            url: `/api/ViTriKhoNew/Get?Action=GetKe&Para1=${$(".nhamay").val()}&Para2=${$(".tenkho").val()}&Para3=${$(".tenkhuvuc").val()}&Para4=a&Para5=a&Para6=a`,
            type: 'GET'
        });
        if (data.length == 0) {
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
                    let eye = `<div> <i style="font-size: 11px; cursor: pointer; border: 1px solid; padding: 0 2px;
                    background: #fff; border-radius: 5px; outline: none; margin-top:0px;display:none" class="fa-solid fa-eye-slash eye-hideSL"></i>
                    </div>`
                    let valueo = `<p style="margin-top:2px;white-space: nowrap;margin-left: 5px;">${o.NameO}</p>`
                    let chuyenIndex = `<i class="fa-solid fa-arrow-right arrowIndex"></i>`

                    let checkAll = `  <input style="margin: 0;" class='checkAll' type="checkbox" />`
                    let cbmO = Math.round(o.CBMTotal * 1000) / 1000
                    let iconPast = `<i class="fa-duotone fa-solid fa-paste pasteIcon"></i>`
                    iconmau = `
                             ${valueo} ${chuyenIndex} <div class="list_p"> ${eye} <p class="CBMTD"> (${cbmO}/${o.CBM})</p></div>`;

                    let htmlnew = ""
                    let htmlnew2 = ""
                    if (cbmO != o.CBM) {
                        // htmlnew = await renderThungInKe(make, matang, o.MaO)
                        //htmlnew2 = await renderThungInKeChiTiet(make, matang, o.MaO)
                    }
                    htmlO += `<td style="text-align: left;vertical-align: top; " data-cbmcl="${cbmO}" data-teno="${o.NameO}" data-make="${make}" data-o="${o.MaO}" data-khoiluong="${o.CBM}" data-tang="${matang}">
                       <div class="list_item">
                           <div class="item_Icon">
                              ${iconmau}
                           </div>
                            <div style="min-width: 200px;">

                           <div class="item_thung"  style="display:none">
                                ${htmlnew}
                           </div>
                        <div class="item_thung1" style="display:${checkHide}"  >
                              ${htmlnew2}
                            </div>
                            </div>
                            </div>
                        </div>
                     </td>`;

                }
                indexT++;
                htmlKe = `<td class="rowMerge"  style="white-space: ; max-width: 16px; min-width: 30px;background: #afe5ff;">
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
            $(".rendertable_list").html(htmlTotal)
            MergeRow()
            renderListThung()
        }
        if (value == 1)
            removeLoading()
        //sumCBM()
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
        let htmlnew2 = await renderThungInKeChiTiet(make, matang, mao)
        $thisTD.find(".item_thung1").html(htmlnew2)
        //let htmlnew = await renderThungInKe(make, matang, mao)
        //$thisTD.find(".item_thung").html(htmlnew)
    })
}
async function renderThungInKeChiTiet(make, matang, mao) {
    const response = await fetch(`/api/ViTriKhoTong/Get?Action=GetDHKho&Para1=${$(".nhamay").val()}&Para2=${$(".tenkho").val()}&Para3=${$(".tenkhuvuc").val()}&Para4=${make}&Para5=${matang}&Para6=${mao}&Para7=a`);
    const data = await response.json();
    let html = '';
    data.forEach(item => {
        html +=
            `
                 <div data-mahang="${item.MaHang}" data-po="${item.PoID}" data-sizeid="${item.SizeID}"  data-madh="${item.MaDH}" data-size="${item.Size}"
                    data-sokien="${item.SLThung}"
                    class="gopitem1">
                    <div class="listSearch">
                            <p > <i class="fa-solid fa-circle icon" style="color: #91d219;"></i>MH: ${item.MaHang}</p>
                             <p ><i class="fa-solid fa-circle icon" style="color: #869d5b;"></i>PO: ${item.Po}</p>
                             <p> <i class="fa-solid fa-circle icon" style="color: #19c8d2;"></i>SK: ${item.SLThung}</p>
                            <div style="display: flex;  align-items: center;"> <p><i class="fa-solid fa-circle icon" style="color: #6219d2;"></i>Size: ${item.Size}</p>
                                <i class="fa-solid fa-eye-slash eye_hideSize"></i>
                    
                            </div>
                    </div>
                 
               
            </div>`
            ;
    });
    return html;
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

function renderTablesWithEqualColumns(html) {
    let container = $(html);
    let maxTdCount = findMaxTdCountFromHtml(html);

    container.find('.tbody').each(function () {
        let table = $(this);
        table.find('tr').each(function () {
            let rowCount = $(this).find('td').length;
            if (rowCount < maxTdCount) {
                let missingTdCount = maxTdCount - rowCount;
                let lastTd = $(this).find('td:last');

                for (let i = 0; i < missingTdCount; i++) {
                    lastTd.attr('colspan', parseInt(lastTd.attr('colspan') || 1) + 1);
                    lastTd.after('<td style="display: none;"></td>');
                }
            }
        });
    });

    return container.html();
}
let $tdTang = ""
let $tdKe = ""
let $tdO = ""
let $tdTenO = ""
$(".rendertable_list").on("click", '.eye_hideSize', function () {
    let arrSize = []
    let $this = $(this).closest(".gopitem1")
    let $thisTd = $this.closest("td")
    let size = $this.data("size")
    let sizeID = $this.data("sizeid")
    let po = $this.data("po")
    let mahang = $this.data("mahang")
    let madh = $this.data("madh")
    let sokien = $this.data("sokien")
    let arr = size.split(';');
    let arrID = sizeID.split(';');
    $tdKe = $thisTd.data("make")
    $tdTang = $thisTd.data("tang")
    $tdO = $thisTd.data("o")
    $tdTenO = $thisTd.data("teno")
    arr.map((item, index) => {
        let object = {
            Size: item,
            MaDH: madh,
            SizeID: arrID[index],
            Po: po,
            MaHang: mahang,
            Display: `MH: ${mahang} - PO: ${po} - Size: ${item}`
        }
        arrSize.push(object)
    })
    let html = ``
    arrSize.map(item => {
        html += `
              <option data-po="${item.Po}" data-sizeid="${item.SizeID}" data-size="${item.Size}"  value="${item.MaDH}">${item.Display}</option>
        `
    })
    $(".searchInSize").html(html)
    $(".searchInSize").select2()
    $('#myModal2').modal('show');
    rederChiTietInO()
})
$(".searchInSize").on("change", function () {
    rederChiTietInO()
})
function rederChiTietInO() {
    let po = $(".searchInSize option:selected").data("po")
    let madh = $(".searchInSize").val()
    let size = $(".searchInSize option:selected").data("sizeid")
    let makhuvuc = $(".tenkhuvuc").val()
    let makho = $(".tenkho").val()
    let madvsx = $(".nhamay").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetListSize&Para1=${makhuvuc}&Para2=${$tdKe}&Para3=${$tdTang}&Para4=${madh}&Para5=${po}&Para6=${size}&Para7=${$tdO}&Para8=${makho}&Para9=${madvsx}`,
        type: 'Get',
        success: function (data) {
            let html = ``
            data.map(item => {
                html += `
                     <tr  style="height:30px">
                            <td>${item.MaHang}</td>
                            <td>${item.Po}</td>
                            <td>${item.TenThung}</td>
                            <td>${item.Size}</td>
                            <td>${item.CBM}</td>
                        </tr>

                    `

            })
            $("#bodyChiTietThungInSize").html(html)
            $(".namoDetail").text(`Chi tiết ô ${$tdTenO}`)
        }


    })
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
function renderDH() {
    let makho = $(".tenkho").val()

    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetDH&Para1=${makho}&Para2=a&Para3=a&Para4=a&Para5=a&Para6=a&Para7=a`,
        type: "Get",
        success: function (data) {

            let html = ` `
            data.map(item => {
                html += `
                    <option data-mahang="${item.MaHang}" data-magop="${item.MaDH}"  value="${item.MaGop}">${item.MaDH} - ${item.MaHang} </option>
                `
            })
            $(".madh").html(html)
            renderPO()
        }

    })
}
$(".madh").on("change", function () {
    renderPO()
})
let dataAll = [];
function renderPO() {
    $(".list_loading").removeClass("active")
    let makho = $(".tenkho").val()
    let maDH = $(".madh").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetAllDs&Para1=${maDH}&Para2=A&Para3=a&Para4=a&Para5=a&Para6=a&Para7=a`,
        type: "Get",
        success: function (data) {
            if (data.length == 0) {
                $(".maPKL").empty()
                $(".color").empty()
                $(".Size").empty()
                $(".item_thung").empty()
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
        return { MaPKL: item.MaPKL, SLThung: item.SLThung, SLSP: item.SLSP };
    });
    var unniqueMap = dataMPLNew.reduce(function (accumulator, item) {
        if (!accumulator[item.MaPKL]) {
            accumulator[item.MaPKL] = { MaPKL: item.MaPKL, SLThung: 0, SLSP: 0 };
        }
        accumulator[item.MaPKL].SLThung += item.SLThung;
        accumulator[item.MaPKL].SLSP += item.SLSP;
        return accumulator;
    }, {});
    var resultArray = Object.values(unniqueMap);
    let html = ` `
    if (resultArray.length == 0) {
        $(".color").empty()
        $(".Size").empty()
        $(".item_thung").empty()
        return
    }
    resultArray.map(item => {
        html += `
                    <option value="${item.MaPKL}">${item.MaPKL} - SK: ${item.SLThung}</option>
                `
    })
    $(".maPKL").html(html)
    renderColorAndDS()
    renderDS()

}
$(".maPKL").on("change", function () {
    renderColorAndDS()
    renderDS()
})
let stthungminSize = ""
let sttthungmaxSize = ""
function renderColorAndDS() {
    $(".list_loading").removeClass("active")
    let pkl = $(".maPKL").val()
    let dataColor = dataAll.filter(x => x.POID == $(".PO").val() && x.MaPKL == pkl)
    let dataColorNew = dataColor.map(item => {
        return { ColorID: item.ColorID, Mau: item.Mau };
    });
    let unniqueMap = dataColorNew.reduce((unique, item) => {
        return unique.some(t => t.ColorID === item.ColorID) ? unique : [...unique, item];
    }, []);
    let html = ` `
    if (unniqueMap.length == 0) {
        $(".Size").empty()
        $(".item_thung").empty()
        $(".list_loading").addClass("active")
        return
    }
    unniqueMap.map(item => {
        html += `
                    <option value="${item.ColorID}">${item.Mau}</option>
                `
    })
    $(".color").html(html)
    renderSize()
}
$(".color").on("change", function () {
    renderSize()
})
function renderDS() {
    $(".list_loading").removeClass("active")
    let pkl = $(".maPKL").val()
    let dataDauSize = dataAll.filter(x => x.POID == $(".PO").val() && x.MaPKL == pkl)
    let dataDauSizeNew = dataDauSize.map(item => {
        return { DauSizeID: item.DauSizeID, DauSize: item.DauSize };
    });
    let unniqueMap = dataDauSizeNew.reduce((unique, item) => {
        return unique.some(t => t.DauSizeID === item.DauSizeID) ? unique : [...unique, item];
    }, []);
    let html = ` `
    if (unniqueMap.length == 0) {
        $(".Size").empty()
        $(".item_thung").empty()
        $(".list_loading").addClass("active")
        return
    }
    unniqueMap.map(item => {
        html += `
                    <option value="${item.DauSizeID}">${item.DauSize}</option>
                `
    })
    $(".dausize").html(html)
    renderSize()
}
$(".dausize").on("change", function () {
    renderSize()
})
function renderSize() {
    let pkl = $(".maPKL").val()
    let dataSize = dataAll.filter(x => x.POID == $(".PO").val() && x.MaPKL == pkl && x.ColorID == $(".color").val() && x.DauSizeID == $(".dausize").val() && x.IsNhapKho == 1)
    let dataSizeNew = dataSize.map(item => ({
        Size: item.Size,
        SizeID: item.SizeID,
        MinSttThung: item.MinSttThung,
        MaxSttThung: item.MaxSttThung,
        IsDeCat: item.isDecat,
        MaKho: item.MaKho
    }))
    let unniqueMap = dataSizeNew.reduce((unique, item) => {
        return unique.some(t => t.SizeID === item.SizeID) ? unique : [...unique, item];
    }, []);
    let html = ` `
    if (unniqueMap.length == 0) {
        $(".Size").empty()
        $(".item_thung").empty()
        $(".list_loading").addClass("active")
        return
    }
    unniqueMap.map(item => {
        html += `
                    <option data-makho="${item.MaKho}" data-isdecat="${item.IsDeCat}" data-stthungmin="${item.MinSttThung}" data-stthungmax="${item.MaxSttThung}" data-size="${item.Size}" value="${item.SizeID}">${item.Size}</option>
                `
    })
    $(".Size").html(html)
    rederSThung()
}
$(".Size").on("change", function () {
    rederSThung()
})
let arrDataDsThung = []

function rederSThung(value) {
    if (value != 1) $(".list_loading").removeClass("active")
    let makho = $(".Size option:selected").data("makho")
    let maDH = $(".madh").val()
    let PO = $(".PO").val()
    let pkl = $(".maPKL").val()
    let sttthungmin = $(".Size option:selected").data("stthungmin")
    let sttthungmax = $(".Size option:selected").data("stthungmax")
    let isdecat = $(".Size option:selected").data("isdecat")
    let colorid = $(".color").val()
    let dausize = $(".dausize").val()
    if (colorid.includes(","))
        colorid = colorid.split(",")[0]
    if (dausize.includes(","))
        dausize = dausize.split(",")[0]

    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetSttThung&Para1=${pkl}&Para2=${PO}&Para3=${maDH}&Para4=${sttthungmin == null ? 0 : sttthungmin}&Para5=${sttthungmax == null ? 0 : sttthungmax}&Para6=${makho}&Para7=${isdecat}&Para8=${colorid}&Para9=${dausize}`,
        type: "Get",
        success: function (data) {

            if (data.length == 0) {
                $(".list_dsThung").find(".item_thung").empty()
                $(".list_loading").addClass("active")
                return
            }
            let check = true;
            if (data[0].SttThung_decat != 0) {
                check = false;
            }
            stthungminSize = check == true ? data[0].SttThung_temp : data[0].SttThung_decat
            sttthungmaxSize = check == true ? data[data.length - 1].SttThung_temp : data[data.length - 1].SttThung_decat
            arrDataDsThung = data;
            renderValueDsthung(data)
            $(".list_loading").addClass("active")
        }
    })
}
function renderValueDsthung(data) {
    let mahang = $(".madh option:selected").data("mahang")
    let colorid = $(".color").val()
    let dausize = $(".dausize").val()
    let ds = $(".dausize option:selected").text()
    let color = $(".color option:selected").text()
    let nameSize = $(".Size option:selected").data("size")
    let po = $(".PO option:selected").text()
    let poid = $(".PO").val()
    let pkl = $(".maPKL").val()
    let madh = $(".madh").val()
    let size = $(".Size").val()
    let html = ``
    let check = true;
    if (data[0].SttThung_decat != 0) {
        check = false;
    }
    console.log(data)
    data.map(item => {
        let khoiluong = item.KhoiLuong == null ? 0 : item.KhoiLuong
        let stt = check == true ? item.SttThung_temp : item.SttThung_decat
        html += `
                 <div data-po="${po}" data-soluong=${item.SoLuong} data-namesize="${nameSize}" data-ds="${ds}" data-color="${color}"  data-poid="${poid}" data-dausizeid="${dausize}" data-colorid="${colorid}" data-mapkl="${pkl}" data-mahang="${mahang}" data-stthung="${item.SttThung}" data-viewthung="${stt}" data-khoiluong='${khoiluong}'
                    data-madh="${madh}"  data-size="${size}" class="gopitem">
                        <p>${stt}</p>
                        <input class="checkItemDSThung" type="checkbox" />
                    </div>
                `
    })
    $(".list_dsThung .item_thung").find(".checkItemDSThung").prop("checked", false)
    $(".checkDSThung").prop("checked", false)
    $(".list_dsThung").find(".item_thung").html(html)
    $(".totalCBM").text(0 + " (CBM)");
}
function filreDataNew() {
    let arrnew = arrDataDsThung
    let tuthung = $(".tuthung").val()
    let denthung = $(".denthung").val()
    if (tuthung == "" || denthung == "") return;
    let check = true;
    if (arrnew[0].SttThung_decat != 0) {
        check = false;
    }
    let stt = check == true ? "SttThung_temp" : "SttThung_decat"
    let filteredArray = arrnew.filter(function (item) {
        let sttThungNumber = parseInt(item[stt], 10);
        return sttThungNumber >= tuthung && sttThungNumber <= denthung;
    });
    renderValueDsthung(filteredArray)
}
$(".searchThung").on("click", ".searchListThung", function () {
    filreDataNew()
})

$(".tuthung").on("change", function () {
    let inputvalue = $(this).val()
    checkValueMinMax(inputvalue, $(this))
})
$(".denthung").on("change", function () {
    let inputvalue = $(this).val()
    checkValueMinMax(inputvalue, $(this))
})
//check value
function checkValueMinMax(input, $this) {
    let valueDenthung = $(".denthung").val()
    let valueTuThung = $(".tuthung").val()
    let sttthungmin = stthungminSize
    let sttthungmax = sttthungmaxSize
    if (Number(input) > sttthungmax) {
        toastr.error(`Vui lòng nhập giá trị từ thùng nhỏ hơn ${sttthungmax}`)
        $this.val("")
        return;
    }
    else if (Number(input) < sttthungmin) {
        toastr.error(`Vui lòng nhập giá trị từ thùng nhỏ hơn ${sttthungmin}`)
        $this.val("")
        return
    }
    if ($this.is(".tuthung") && Number(input) > Number(valueDenthung) && valueDenthung != "") {
        toastr.error("Vui lòng nhập giá trị từ thùng nhỏ hơn giá trị đến thùng")
        $this.val("")
        return;
    }
    if ($this.is(".denthung") && Number(input) < Number(valueTuThung) && valueTuThung != "") {
        toastr.error("Vui lòng nhập giá trị đến thùng lớn hơn giá trị từ thùng")
        $this.val("")
        return;
    }
}
$(".selectDs").select2()
$(".searchThung").on("change", ".checkDSThung", function () {
    let $this = $(this)
    if ($this.prop("checked")) eachListThung(true)
    else eachListThung(false)

})
let tongCBM = 0
function eachListThung(flagCheck) {
    let $tongCBM = 0;
    $(".list_dsThung .gopitem").each(function () {
        let $div = $(this);
        let $divKL = flagCheck == true ? parseFloat($div.data("khoiluong")) : 0;
        $div.find(".checkItemDSThung").prop("checked", flagCheck);
        $tongCBM += $divKL;
    });
    $tongCBM = Math.round($tongCBM * 10000) / 10000;
    tongCBM = $tongCBM;
    $(".totalCBM").text($tongCBM + " (CBM)");
}
$(".list_dsThung ").on("change", ".checkItemDSThung", function () {
    let $this = $(this)
    let khoiluong = $this.closest("div").data("khoiluong")
    if (khoiluong == 0) {
        Swal.fire({
            title: 'Vui lòng khai báo CBM cho thùng!',
            icon: 'warning',
            showConfirmButton: false,
            timer: 2000, // Thời gian hiển thị thông báo (1500 ms = 1.5 giây)
            timerProgressBar: true, // Hiển thị thanh tiến trình của timer
            allowOutsideClick: false,
            backdrop: true,
            didOpen: () => {
                Swal.showLoading(); // Hiển thị thanh tiến trình khi thông báo đang được hiển thị
            }
        });
        $this.prop("checked", false)
        return;
    }
    if ($this.prop("checked"))
        tongCBM += parseFloat(khoiluong)
    else
        tongCBM -= parseFloat(khoiluong)
    tongCBM = Math.round(tongCBM * 10000) / 10000;
    $(".totalCBM").text(tongCBM + " (CBM)");
})
$(".iconNapLap").on("click", 'i', function () {
    rederSThung()
})
$(".rendertable_list ").on("click", ".eye-hideSL", function () {
    $(this).toggleClass('fa-eye-slash fa-eye');
    let $itemthung = $(this).closest("td").find(".item_thung")
    if ($(this).hasClass("fa-eye-slash")) {
        $itemthung.hide()
    } else {
        $itemthung.show()
    }
})

function getPasteTD($this) {
    let $tongCBM = 0;
    let $tongCBMTD = 0;
    let arrPaste = []

    let makho = $(".tenkho").val()
    let mao = $this.data("o")
    let matang = $this.data('tang')
    let make = $this.data("make")
    let makhuvuc = $(".tenkhuvuc").val()
    let madvsx = $(".nhamay").val()
    let user = localStorage.getItem("username1")


    $(".list_dsThung .gopitem").each(function () {
        let $div = $(this);
        if (!$div.find(".checkItemDSThung").prop("checked")) return
        let $trStt = $div.data("stthung")
        let $trView = $div.data("viewthung")
        let pkl = $div.data("mapkl")
        let mahang = $div.data("mahang")
        let colorid = $div.data("colorid")
        let dausize = $div.data("dausizeid")
        let poid = $div.data("poid")
        let madh = $div.data("madh")
        let size = $div.data("size")
        let ds = $div.data("ds")
        let color = $div.data("color")
        let nameSize = $div.data("namesize")
        let po = $div.data("po")
        let $divKL = parseFloat($div.data("khoiluong"));
        $tongCBM += $divKL;
        let object = {
            MaDH: madh,
            MaHang: mahang,
            Ma_PKL: pkl,
            MaDVSX: madvsx,
            SizeID: size,
            Size: nameSize,
            PoID: poid,
            Po: po,
            DauSizeID: dausize,
            DauSize: ds,
            ColorID: colorid,
            Color: color,
            MaKhuVuc: makhuvuc,
            MaKho: makho,
            MaKe: make,
            MaTang: matang,
            MaO: mao,
            NVNhap: user,
            SttThung: $trStt,
            SttThungView: $trView,
            KhoiLuong: $divKL
        };
        arrPaste.push(object)
    });
    $tongCBM = Math.round($tongCBM * 10000) / 10000;
    $(".rendertable_list table").find("td .gopitem").each(function () {
        let $div = $(this);
        if (!$div.find(".checkItemDSThung").prop("checked")) return;
        let $trStt = $div.data("stthung");
        let $trView = $div.data("viewthung");
        let $divKL = parseFloat($div.data("khoiluong"));
        let pkl = $div.data("mapkl")
        let mahang = $div.data("mahang")
        let colorid = $div.data("colorid")
        let dausize = $div.data("dausizeid")
        let poid = $div.data("poid")
        let madh = $div.data("madh")
        let size = $div.data("size")
        let ds = $div.data("ds")
        let color = $div.data("color")
        let nameSize = $div.data("namesize")
        let po = $div.data("po")
        $tongCBMTD += $divKL;
        let object = {
            MaDH: madh,
            MaHang: mahang,
            Ma_PKL: pkl,
            MaDVSX: madvsx,
            SizeID: size,
            Size: nameSize,
            PoID: poid,
            Po: po,
            DauSizeID: dausize,
            DauSize: ds,
            ColorID: colorid,
            Color: color,
            MaKhuVuc: makhuvuc,
            MaKho: makho,
            MaKe: make,
            MaTang: matang,
            MaO: mao,
            NVNhap: user,
            SttThung: $trStt,
            SttThungView: $trView,
            KhoiLuong: $divKL
        };
        arrPaste.push(object);
        $div.remove()
    });
    if (arrPaste.length == 0) return;
    $tongCBM += $tongCBMTD
    let CBMConLai = $this.data("khoiluong")
    if ($tongCBM > CBMConLai) {
        toastr.error("Vui lòng CBM tổng các thùng nhỏ hơn CBM của ô")
        return;
    }
    let $itemthung = $this.closest("td").find(".item_thung")
    $itemthung.show()
    $this.find(".eye-hideSL").removeClass('fa-eye-slash');
    $this.find(".eye-hideSL").addClass('fa-eye');
    //$(".item_checkboxdetail").prop("checked",false)
    renderPasteTD($this, arrPaste)
}

$(".rendertable_list ").on("click", ".pasteIcon", function () {
    let $this = $(this).closest("td")
    getPasteTD($this)
})

$(".rendertable_list").on("click", ".checkAll", function () {
    let $td = $(this).closest("td")
    if ($td.find(".gopitem").length == 0) {
        $(this).prop("checked", false)
        return;
    }
    showConfirmationSeal($(this))
})


$(".item_tuthung").on("change", function () {
    let inputvalue = $(this).val()
    checkValueMinMaxViTri(inputvalue, $(this))
})
$(".item_denthung").on("change", function () {
    let inputvalue = $(this).val()
    checkValueMinMaxViTri(inputvalue, $(this))
})
function checkValueMinMaxViTri(input, $this) {
    let valueDenthung = $(".item_denthung").val()
    let valueTuThung = $(".item_tuthung").val()
    if (input > $divDenThung) {
        toastr.error(`Vui lòng nhập giá trị từ thùng nhỏ hơn ${$divDenThung}`)
        $this.val("")
        return;
    }
    else if (input < $divTuThung) {
        toastr.error(`Vui lòng nhập giá trị từ thùng nhỏ hơn ${$divTuThung}`)
        $this.val("")
        return
    }
    if ($this.is(".item_tuthung") && Number(input) > Number(valueDenthung) && valueDenthung != "") {
        toastr.error("Vui lòng nhập giá trị từ thùng nhỏ hơn giá trị đến thùng")
        $this.val("")
        return;
    }
    if ($this.is(".item_denthung") && Number(input) < Number(valueTuThung) && valueTuThung != "") {
        toastr.error("Vui lòng nhập giá trị đến thùng lớn hơn giá trị từ thùng")
        $this.val("")
        return;
    }
}
async function renderThungInKe(make, matang, mao) {
    const response = await fetch(`/api/ViTriKhoTong/Get?Action=GetThungKho&Para1=${$(".nhamay").val()}&Para2=${$(".tenkho").val()}&Para3=${$(".tenkhuvuc").val()}&Para4=${make}&Para5=${matang}&Para6=${mao}&Para7=a`);
    const data = await response.json();
    let html = '';
    data.forEach((item, index) => {
        html +=
            `
                 <div data-po="${item.Po}" data-namesize="${item.Size}" data-ds="${item.DauSize}" data-color="${item.Color}" data-poid="${item.PoID}" data-dausizeid="${item.DauSizeID}" data-colorid="${item.ColorID}" data-mapkl="${item.Ma_PKL}" data-mahang="${item.MaHang}"
                data-stthung="${item.SttThung}" data-viewthung="${item.TenThung}" data-magop="${item.MaGop}" data-khoiluong='${item.CBM}' data-madh="${item.MaDH}" data-size="${item.SizeID}"
                class="gopitem">
                <p>${item.TenThung}</p>
                <input class="checkItemDSThung sttthung${item.SttThung}${item.MaGop}${item.Po}${item.Ma_PKL}" type = "checkbox" />
            </div>`
            ;
    });
    //$(".item_checkboxdetail").prop("checked", false)
    return html;
}
function sumCBM() {

    $(".rendertable_list table").find("td").each(function () {
        let $this = $(this);
        let cbmnew = 0;
        let cbmold = parseFloat($this.data("khoiluong"));

        if ($this.find('.gopitem').length > 0) {
            $this.find('.gopitem').each(function () {
                let $div = $(this);
                let $divKL = parseFloat($div.data("khoiluong")) || 0;
                cbmnew += $divKL;
            });
            let cbmtong = cbmold - cbmnew;
            cbmtong = Math.round(cbmtong * 1000) / 1000;

            $this.find(".CBMTD").text(`(${cbmtong}/${cbmold})`);
        } else {
            $this.find(".CBMTD").text(`(${cbmold}/${cbmold})`);
        }

    });
}
function sumCBMBoxPage1() {
    let makevalue = ""
    let tangvalue = ""
    let ovalue = ""
    makevalue = $(".page1_make").val()
    tangvalue = $(".page1_matang").val()
    ovalue = $(".page1_mao").val()

    $(".rendertable_list table").find("td").each(function () {
        let $this = $(this);
        let make = $this.data("make")
        let matang = $this.data("tang")
        let mao = $this.data("o")
        if (make == makevalue && matang == tangvalue && mao == ovalue) {
            CBMTotalPage1 = $this.data("khoiluong")
            let cbmnew = 0;
            if ($(".checkBoxlistDi .checkBoxItem").length > 0) {
                $(".checkBoxlistDi .checkBoxItem").each(function () {
                    let $div = $(this);
                    let $divKL = parseFloat($div.data("khoiluong")) || 0;
                    cbmnew += $divKL
                })
                let cbmtong = CBMTotalPage1 - cbmnew;
                cbmtong = Math.round(cbmtong * 1000) / 1000;
                $(".CBMPage1").text(`(${cbmtong}/${CBMTotalPage1})`)
                $(".CBMPage1").attr("data-cbm", cbmtong)
                CBMConLaiPage1 = cbmtong
            }
            else {
                CBMConLaiPage1 = CBMTotalPage1
                $(".CBMPage1").text(`(${CBMTotalPage1}/${CBMTotalPage1})`)
                $(".CBMPage1").attr("data-cbm", CBMTotalPage1)
            }
        }
    });
}
let CBMConLaiPage1 = 0;
let CBMConLaiPage2 = 0
function sumCBMBoxPage2() {
    let makevalue = ""
    let tangvalue = ""
    let ovalue = ""
    makevalue = $(".page2_make").val()
    tangvalue = $(".page2_matang").val()
    ovalue = $(".page2_mao").val()
    $(".rendertable_list table").find("td").each(function () {
        let $this = $(this);
        let make = $this.data("make")
        let matang = $this.data("tang")
        let mao = $this.data("o")
        if (make == makevalue && matang == tangvalue && mao == ovalue) {
            CBMTotalPage2 = $this.data("khoiluong")
            let cbmnew = 0;
            if ($(".checkBoxlistDen .checkBoxItem").length > 0) {
                $(".checkBoxlistDen .checkBoxItem").each(function () {
                    let $div = $(this);
                    let $divKL = parseFloat($div.data("khoiluong")) || 0;
                    cbmnew += $divKL
                })
                let cbmtong = CBMTotalPage2 - cbmnew;
                cbmtong = Math.round(cbmtong * 1000) / 1000;
                CBMConLaiPage2 = cbmtong
                $(".CBMPage2").text(`(${cbmtong}/${CBMTotalPage2})`)
                $(".CBMPage2").attr("data-cbm", cbmtong)
            }
            else {
                CBMConLaiPage2 = CBMTotalPage2
                $(".CBMPage2").text(`(${CBMTotalPage2}/${CBMTotalPage2})`)
                $(".CBMPage2").attr("data-cbm", CBMTotalPage2)
            }
        }
    });

}
$(".item_checkboxdetail").on("change", function () {
    let $this = $(this)
    if ($this.prop("checked"))
        $(".rendertable_list").find(".checkItemDSThung").hide()
    else $(".rendertable_list").find(".checkItemDSThung").show()
})
$(".rendertable_list").on("click", ".gopitem", function () {
    let $this = $(this)
    popupDetailThung($this)
})
$(".checkBoxlistDen ").on("click", ".checkBoxItem", function () {
    //let $this = $(this)
    //popupDetailThung($this)
})
function popupDetailThung($this) {
    if ($(".item_checkboxdetail").prop("checked")) {
        let po = $this.data("poid");
        let dausize = $this.data("dausizeid");
        let pkl = $this.data("mapkl");
        let color = $this.data("colorid");
        let mahang = $this.data("mahang");
        let size = $this.data("namesize");
        let cbm = $this.data("khoiluong")
        let tenthung = $this.data("viewthung")
        let soluong = $this.data("soluong")
        let html = `
         <tr  style="height:30px">
                            <td>${mahang}</td>
                            <td>${po}</td>
                            <td>${pkl}</td>
                            <td>${size}</td>
                            <td>${cbm}</td>
                            <td>${soluong}</td>
                            <td>${tenthung}</td>
                        </tr>

        `
        $("#bodyChiTietThung").html(html)
        $('#myModal').modal('show');
    }
}
$(".viewAll").on("click", function () {
    let $this = $(this)
    $this.find(".eye_all").toggleClass('fa-eye-slash fa-eye');
    let $itemthung = $(".rendertable_list").find(".item_thung1")
    $(".rendertable_list").find(".eye-hideSL").removeClass('fa-eye-slash');
    if ($this.find(".eye_all").hasClass("fa-eye-slash")) {
        $itemthung.hide()
        $(".rendertable_list").find(".eye-hideSL").addClass("fa-eye-slash")
    } else {
        $itemthung.show()
        $(".rendertable_list").find(".eye-hideSL").addClass("fa-eye")
    }

})
$(".list_tabIndex").on("click", "button", function () {
    $(".list_tabIndex").find("button").removeClass("active")
    $(this).addClass("active")
    var tab_id = $(this).data('tab');
    $('.tab-content').removeClass('active');
    $(`#${tab_id}`).addClass('active');
    if (tab_id == "tab-2") {
        $('.viewAll').removeClass('active2');
        $('.changeo ').removeClass('active2');
    }
    else {

        $('.viewAll').addClass('active2');
        $('.changeo ').addClass('active2');
    }
})
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
        title: 'Lựa chọn chức năng',
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
            setValueAndTrigger(".page1_day", tenkhuvuc)
                .then(() => setValueAndTrigger(".page1_make", make))
                .then(() => setValueAndTrigger(".page1_matang", matang))
                .then(() => setValueAndTrigger(".page1_mao", mao))
                .then(() => {
                    $(".page2").trigger("click");
                });
        }

    });
    //$(".swal2-cancel").on("click", function () {
    //    indexViTri = 2
    //    setValueAndTrigger(".page2_make", make)
    //        .then(() => setValueAndTrigger(".page2_matang", matang))
    //        .then(() => setValueAndTrigger(".page2_mao", mao))
    //        .then(() => {
    //            $(".page2").trigger("click");
    //        });
    //})


})
function setValueAndTrigger(selector, value) {
    return new Promise((resolve) => {
        $(selector).val(value).trigger("change");
        setTimeout(() => resolve(), 300);
    });
}

function rederPageKe1() {
    let dvsx = $(".nhamay").val()
    let tenkho = $(".tenkho").val()
    let tenkhuvuc = $(".page1_day").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetKe&Para1=${dvsx}&Para2=${tenkho}&Para3=${tenkhuvuc}&Para4=a&Para5=a&Para6=a&Para7=$a`,
        type: "Get",
        success: function (data) {
            let html = ` <option></option>`
            data.map(item => {
                html += `
                        <option  value="${item.MaKe}">${item.TenKe}</option>
                `
            })
            $(".page1_make").html(html)
            rederPage1Tang()
        }

    })
}
function rederPageKe2() {
    let dvsx = $(".nhamay").val()
    let tenkho = $(".tenkho").val()
    let tenkhuvuc = $(".page2_day").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetKe&Para1=${dvsx}&Para2=${tenkho}&Para3=${tenkhuvuc}&Para4=a&Para5=a&Para6=a&Para7=$a`,
        type: "Get",
        success: function (data) {
            let html = ` <option></option>`
            data.map(item => {
                html += `
                        <option  value="${item.MaKe}">${item.TenKe}</option>
                `
            })
            $(".page2_make").html(html)
            rederPage2Tang()
        }

    })
}
function rederPage1Tang() {
    let dvsx = $(".nhamay").val()
    let tenkho = $(".tenkho").val()
    let tenkhuvuc = $(".page1_day").val()
    let make = $(".page1_make").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetTang&Para1=${dvsx}&Para2=${tenkho}&Para3=${tenkhuvuc}&Para4=${make}&Para5=a&Para6=a&Para7=a`,
        type: "Get",
        success: function (data) {

            let html = ` <option></option>`
            data.map(item => {
                html += `
                        <option  value="${item.MaTang}">${item.TenTang}</option>
                `
            })
            $(".page1_matang").html(html)
            rederPage1O()
        }

    })
}
function rederPage2Tang() {
    let dvsx = $(".nhamay").val()
    let tenkho = $(".tenkho").val()
    let tenkhuvuc = $(".page2_day").val()
    let make = $(".page2_make").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetTang&Para1=${dvsx}&Para2=${tenkho}&Para3=${tenkhuvuc}&Para4=${make}&Para5=a&Para6=a&Para7=a`,
        type: "Get",
        success: function (data) {
            let html = ` <option></option>`
            data.map(item => {
                html += `
                        <option  value="${item.MaTang}">${item.TenTang}</option>
                `
            })
            $(".page2_matang").html(html)
            rederPage2O()
        }

    })
}
function rederPage1O() {
    let dvsx = $(".nhamay").val()
    let tenkho = $(".tenkho").val()
    let tenkhuvuc = $(".page1_day").val()
    let make = $(".page1_make").val()
    let tang = $(".page1_matang").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetO&Para1=${dvsx}&Para2=${tenkho}&Para3=${tenkhuvuc}&Para4=${make}&Para5=${tang}&Para6=a&Para7=$a`,
        type: "Get",
        success: function (data) {
            let html = ` `
            data.map(item => {
                html += `
                        <option data-cbmo="${item.CBM}"  value="${item.MaO}">${item.NameO}</option>
                `
            })
            $(".page1_mao").html(html)
            BoxDiItem()
        }

    })
}
function rederPage2O() {
    let dvsx = $(".nhamay").val()
    let tenkho = $(".tenkho").val()
    let tenkhuvuc = $(".page2_day").val()
    let make = $(".page2_make").val()
    let tang = $(".page2_matang").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetO&Para1=${dvsx}&Para2=${tenkho}&Para3=${tenkhuvuc}&Para4=${make}&Para5=${tang}&Para6=a&Para7=$a`,
        type: "Get",
        success: function (data) {
            let html = ``

            data.map(item => {
                html += `
                        <option data-cbmo="${item.CBM}"  value="${item.MaO}">${item.NameO}</option>
                `
            })
            $(".page2_mao").html(html)
            BoxDenItem()
            if (flagCheckNew)
                renderValueNew()
        }

    })
}
indexCheck = 1;
$(".page2_make").on("change", function () {
    rederPage2Tang()
})
$(".page2_matang").on("change", function () {
    rederPage2O()
})
$(".page2_mao").on("change", function () {
    BoxDenItem()
})
$(".page1_make").on("change", function () {
    rederPage1Tang()
})
$(".page1_matang").on("change", function () {
    rederPage1O()
})
$(".page1_mao").on("change", function () {
    BoxDiItem()
})
let CBMTotalPage1 = ""
let CBMTotalPage2 = ""
function BoxDiItem() {
    let dvsx = $(".nhamay").val()
    let tenkho = $(".tenkho").val()
    let tenkhuvuc = $(".page1_day").val()
    let make = $(".page1_make").val()
    let tang = $(".page1_matang").val()
    let o = $(".page1_mao").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetThungKho&Para1=${dvsx}&Para2=${tenkho}&Para3=${tenkhuvuc}&Para4=${make}&Para5=${tang}&Para6=${o}&Para7=a`,
        type: "Get",
        success: function (data) {
            let html = '';
            let cbmoPage1 = 0;
            data.forEach((item, index) => {
                cbmoPage1 += item.CBM
                html += `
                <div class="checkBoxItem" data-soluong="${item.SoLuong}"  data-po="${item.Po}" data-namesize="${item.Size}" data-ds="${item.DauSize}" data-color="${item.Color}"  data-poid="${item.PoID}" data-dausizeid="${item.DauSizeID}" data-colorid="${item.ColorID}" data-mapkl="${item.Ma_PKL}" data-mahang="${item.MaHang}"
                data-stthung="${item.SttThung}" data-viewthung="${item.TenThung}" data-magop="${item.MaGop}" data-khoiluong='${item.CBM}' data-madh="${item.MaDH}" data-size="${item.SizeID}"">
                                <p>${item.TenThung}</p>
                                <input class="sttthung${item.SttThung}${item.MaGop}${item.Po}${item.Ma_PKL}" type="checkbox" />
                            </div>
            `

            });
            $(".checkBoxlistDi").html(html)
            $(".rendertable_list").find(".checkItemDSThung ").prop("checked", false)
            let cbmTongPage1 = $(".page1_mao option:selected").data("cbmo")

            let CBMCLPage1 = cbmTongPage1 - cbmoPage1
            CBMCLPage1 = Math.round(CBMCLPage1 * 10000) / 10000;

            if (cbmTongPage1 != null) {
                $(".CBMPage1").html(`(<span class="page_cbmcl">${CBMCLPage1}</span>/<span class="page_cbmt">${cbmTongPage1}</span>)`)
                $(".CBMPage1").data("cbm", CBMCLPage1)
            }
            else {
                $(".CBMPage1").html(`(<span class="page_cbmcl">0</span>/<span class="page_cbmt">0</span>)`)
                $(".CBMPage1").data("cbm", 0)
            }
        }
    })

}
function BoxDenItem() {
    let dvsx = $(".nhamay").val()
    let tenkho = $(".tenkho").val()
    let tenkhuvuc = $(".page2_day").val()
    let make = $(".page2_make").val()
    let tang = $(".page2_matang").val()
    let o = $(".page2_mao").val()
    let check = ' <input class="sttthunga", type="checkbox" />'
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetThungKho&Para1=${dvsx}&Para2=${tenkho}&Para3=${tenkhuvuc}&Para4=${make}&Para5=${tang}&Para6=${o}&Para7=a`,
        type: "Get",
        success: function (data) {
            let html = '';
            let check = `<input class="checkItemDSThung" type="checkbox" />`
            let cbmoPage2 = 0;
            data.forEach((item, index) => {
                cbmoPage2 += item.CBM
                html += `
               <div class="checkBoxItem" data-soluong="${item.SoLuong}"  data-po="${item.Po}" data-namesize="${item.Size}" data-ds="${item.DauSize}" data-color="${item.Color}" data-poid="${item.PoID}" data-dausizeid="${item.DauSizeID}" data-colorid="${item.ColorID}" data-mapkl="${item.Ma_PKL}" data-mahang="${item.MaHang}"
                data-stthung="${item.SttThung}" data-viewthung="${item.TenThung}" data-magop="${item.MaGop}" data-khoiluong='${item.CBM}' data-madh="${item.MaDH}" data-size="${item.SizeID}">
                                <p>${item.TenThung}</p>
                                <input class="sttthung${item.SttThung}${item.MaGop}${item.Po}${item.Ma_PKL}" type="checkbox" />
                            </div>
            `
            });
            $(".checkBoxlistDen").html(html)
            let cbmTongPage2 = $(".page2_mao option:selected").data("cbmo")

            let CBMCLPage2 = cbmTongPage2 - cbmoPage2
            CBMCLPage2 = Math.round(CBMCLPage2 * 10000) / 10000;
            if (cbmTongPage2 != null) {
                $(".CBMPage2").html(`(<span class="page_cbmcl">${CBMCLPage2}</span>/<span class="page_cbmt">${cbmTongPage2}</span>)`)
                $(".CBMPage2").data("cbm", CBMCLPage2)
            }
            else {
                $(".CBMPage2").html(`(<span class="page_cbmcl"> 0</span>/<span class="page_cbmt">0</span>)`)
                $(".CBMPage2").data("cbm", 0)
            }

        }

    })
}
let $thisDen = ""
let $thisDi = ""
let indexButton = "";
function eachThisDen() {
    let makevalue = ""
    let tangvalue = ""
    let ovalue = ""
    if (indexButton == 1) {
        makevalue = $(".page2_make").val()
        tangvalue = $(".page2_matang").val()
        ovalue = $(".page2_mao").val()
    }
    else {
        makevalue = $(".page1_make").val()
        tangvalue = $(".page1_matang").val()
        ovalue = $(".page1_mao").val()
    }
    $(".rendertable_list table").find("td").each(function () {
        let $this = $(this);
        let make = $this.data("make")
        let matang = $this.data("tang")
        let mao = $this.data("o")
        if (make == makevalue && matang == tangvalue && mao == ovalue) {
            $thisDen = $this
        }
    });
}

function eachThisDi() {
    let makevalue = ""
    let tangvalue = ""
    let ovalue = ""
    if (indexButton == 1) {
        makevalue = $(".page1_make").val()
        tangvalue = $(".page1_matang").val()
        ovalue = $(".page1_mao").val()
    }
    else {
        makevalue = $(".page2_make").val()
        tangvalue = $(".page2_matang").val()
        ovalue = $(".page2_mao").val()
    }
    $(".rendertable_list table").find("td").each(function () {
        let $this = $(this);
        let make = $this.data("make")
        let matang = $this.data("tang")
        let mao = $this.data("o")
        if (make == makevalue && matang == tangvalue && mao == ovalue) {
            $thisDi = $this
        }
    });
}

$(".pasteIconPage").on("click", function () {

})
function setDSThung() {
    let $tongCBM = 0;
    let $tongCBMTD = 0;
    let arrPaste = []

    let makho = $(".tenkho").val()
    let mao = $thisDen.data("o")
    let matang = $thisDen.data('tang')
    let make = $thisDen.data("make")
    let makhuvuc = $(".tenkhuvuc").val()
    let madvsx = $(".nhamay").val()
    let user = localStorage.getItem("username1")
    $(".list_dsThung .gopitem").each(function () {
        let $div = $(this);
        if (!$div.find(".checkItemDSThung").prop("checked")) return
        let $trStt = $div.data("stthung")
        let $trView = $div.data("viewthung")
        let pkl = $div.data("mapkl")
        let mahang = $div.data("mahang")
        let colorid = $div.data("colorid")
        let dausize = $div.data("dausizeid")
        let poid = $div.data("poid")
        let madh = $div.data("madh")
        let size = $div.data("size")
        let ds = $div.data("ds")
        let color = $div.data("color")
        let nameSize = $div.data("namesize")
        let po = $div.data("po")
        let $divKL = parseFloat($div.data("khoiluong"));
        $tongCBM += $divKL;
        let object = {
            MaDH: madh,
            MaHang: mahang,
            Ma_PKL: pkl,
            MaDVSX: madvsx,
            SizeID: size,
            Size: nameSize,
            PoID: poid,
            Po: po,
            DauSizeID: dausize,
            DauSize: ds,
            ColorID: colorid,
            Color: color,
            MaKhuVuc: makhuvuc,
            MaKho: makho,
            MaKe: make,
            MaTang: matang,
            MaO: mao,
            NVNhap: user,
            SttThung: $trStt,
            SttThungView: $trView,
            KhoiLuong: $divKL
        };
        arrPaste.push(object)
    });
    $tongCBM = Math.round($tongCBM * 10000) / 10000;
    //$(".rendertable_list table").find("td .gopitem").each(function () {
    //    let $div = $(this);
    //    if (!$div.find(".checkItemDSThung").prop("checked")) return;
    //    let $trStt = $div.data("stthung");
    //    let $trView = $div.data("viewthung");
    //    let $divKL = parseFloat($div.data("khoiluong"));
    //    let pkl = $div.data("mapkl")
    //    let mahang = $div.data("mahang")
    //    let colorid = $div.data("colorid")
    //    let dausize = $div.data("dausizeid")
    //    let poid = $div.data("poid")
    //    let madh = $div.data("madh")
    //    let size = $div.data("size")
    //    let ds = $div.data("ds")
    //    let color = $div.data("color")
    //    let nameSize = $div.data("namesize")
    //    let po = $div.data("po")
    //    $tongCBMTD += $divKL;
    //    let object = {
    //        MaDH: madh,
    //        MaHang: mahang,
    //        Ma_PKL: pkl,
    //        MaDVSX: madvsx,
    //        SizeID: size,
    //        Size: nameSize,
    //        PoID: poid,
    //        Po: po,
    //        DauSizeID: dausize,
    //        DauSize: ds,
    //        ColorID: colorid,
    //        Color: color,
    //        MaKhuVuc: makhuvuc,
    //        MaKho: makho,
    //        MaKe: make,
    //        MaTang: matang,
    //        MaO: mao,
    //        NVNhap: user,
    //        SttThung: $trStt,
    //        SttThungView: $trView,
    //        KhoiLuong: $divKL
    //    };
    //    arrPaste.push(object);
    //    $div.remove()
    //});
    if (arrPaste.length == 0) return;
    $tongCBM += $tongCBMTD
    let CBMConLai = $thisDen.data("khoiluong")
    if ($tongCBM > CBMConLai) {
        toastr.error("Vui lòng CBM tổng các thùng nhỏ hơn CBM của ô")
        return;
    }
    //renderPasteTD($thisDen, arrPaste)
}
function postViTriKho(arrPaste) {
    $.ajax({
        type: "POST",
        url: "/api/ViTriKhoTong/Post",
        data: JSON.stringify(arrPaste),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            rederSThung(1)
            renderKe(0)

        }
    });
}

function renderPasteTD($this, data) {
    let html = ``
    data.map(item => {
        html +=
            `
                 <div data-po="${item.Po}" data-namesize="${item.Size}" data-ds="${item.DauSize}" data-color="${item.Color}"  data-poid="${item.PoID}" data-dausizeid="${item.DauSizeID}" data-colorid="${item.ColorID}" data-mapkl="${item.Ma_PKL}" data-mahang="${item.MaHang}"
                data-stthung="${item.SttThung}" data-viewthung="${item.SttThungView}" data-magop="${item.MaGop}" data-khoiluong='${item.KhoiLuong}' data-madh="${item.MaDH}" data-size="${item.SizeID}"
                class="gopitem">
                <p>${item.SttThungView}</p>
                <input class="checkItemDSThung sttthung${item.SttThung}${item.MaGop}${item.Po}${item.Ma_PKL}" type = "checkbox" />
            </div>`
            ;
    })
    $this.find(".item_thung").append(html)
    //postViTriKho(data)
    $(".rendertable_list").find(".checkAll").prop("checked", false)
}
$(".checkBoxlistDi").on("change", "input", function myfunction() {
    let $class = $(this).attr("class")
    if ($(this).prop("checked")) {
        checkInput($class, $thisDi, true)
    }
    else {
        checkInput($class, $thisDi, false)
    }
})
$(".checkBoxlistDen").on("change", "input", function myfunction() {
    let $class = $(this).attr("class")
    if ($(this).prop("checked")) {
        checkInput($class, $thisDi, true)
    }
    else {
        checkInput($class, $thisDi, false)
    }
})
function checkInput($classvalue, $thisDivalue, flagCheck) {
    $(".rendertable_list").find(`.${$classvalue}`).prop("checked", flagCheck)
}
let indexCheckChange = 0;
$(".chuyenList").on("click", ".left_right", function () {
    let makePage1 = $(".page1_make").val()
    let tangPage1 = $(".page1_matang").val()
    let oPage1 = $(".page1_mao").val()
    let makePage2 = $(".page2_make").val()
    let tangPage2 = $(".page2_matang").val()
    let oPage2 = $(".page2_mao").val()
    if (makePage1 == "" && makePage2 == "") return
    changeViTriPage1(makePage1, tangPage1, oPage1)
    changeViTriPage2(makePage2, tangPage2, oPage2)
})
$(".chuyenList").on("click", ".iconpage1", function () {
    let makePage1 = $(".page1_make").val()
    let tangPage1 = $(".page1_matang").val()
    let oPage1 = $(".page1_mao").val()
    let makePage2 = $(".page2_make").val()
    let tangPage2 = $(".page2_matang").val()
    let oPage2 = $(".page2_mao").val()
    let dayPage1 = $(".page1_day").val()
    let dayPage2 = $(".page2_day").val()
    if (dayPage1 == dayPage2 && makePage1 == makePage2 && tangPage1 == tangPage2 && oPage1 == oPage2) {
        toastr.error("Ô bị trùng vui lòng chọn ô khác")
        return
    } else {

        if (oPage2 == null) {
            toastr.error("Vui lòng chọn đầy đủ vị trí")
        } else {
            ////indexButton = 1
            checkBoxListItem(1)
            //chaneSttThung()
            //let $thisRemove = $(".checkBoxlistDi .checkBoxItem")
            //let $thisAdd = $(".checkBoxlistDen ")
            //removeItemThisDi($thisRemove, $thisAdd, 1)
            //setTimeout(function () {
            //    let $thisDiRemove = ""
            //    if ($thisDi != "")
            //        $thisDiRemove = $thisDi.find(".gopitem")

            //    let $thisDenAdd = $thisDen
            //    removeItemThisDi($thisDiRemove, $thisDenAdd, 2)
            //}, 1000)
        }
    }

})

$(".chuyenList").on("click", ".iconpage2", function () {
    let makePage1 = $(".page1_make").val()
    let tangPage1 = $(".page1_matang").val()
    let oPage1 = $(".page1_mao").val()
    let makePage2 = $(".page2_make").val()
    let tangPage2 = $(".page2_matang").val()
    let oPage2 = $(".page2_mao").val()
    let dayPage1 = $(".page1_day").val()
    let dayPage2 = $(".page2_day").val()
    if (dayPage1 == dayPage2 && makePage1 == makePage2 && tangPage1 == tangPage2 && oPage1 == oPage2) {
        toastr.error("Ô bị trùng vui lòng chọn ô khác")
        return
    } else {
        if (oPage1 == null) {
            toastr.error("Vui lòng chọn đầy đủ vị trí")
        } else {
            //indexButton = 2
            checkBoxListItem(2)
            //chaneSttThung()
            //let $thisRemove = $(".checkBoxlistDen .checkBoxItem")
            //let $thisAdd = $(".checkBoxlistDi ")
            //removeItemThisDi($thisRemove, $thisAdd, 1)
            ////setTimeout(function () {
            ////    let $thisDiRemove = ""
            ////    if ($thisDi != "")
            ////        $thisDiRemove = $thisDi.find(".gopitem")
            ////    let $thisDenAdd = $thisDen
            ////    removeItemThisDi($thisDiRemove, $thisDenAdd, 2)
            ////}, 1000)
        }

    }


})
//function removeItemThisDi($thisRemove, $thisAdd, valitem) {
//    let $thisNew = $thisRemove
//    checkBoxListItem($thisNew, $thisAdd, valitem)
//}
function checkBoxListItem(/*$thisNew, $thisAdd, valitem*/value) {
    let CBMTongPage = 0
    let arrThis = []
    let $tongCBMTD = 0;
    let $tongCBM = 0
    let makho = $(".tenkho").val()
    let makhuvuc = ""
    let madvsx = $(".nhamay").val()
    let user = localStorage.getItem("username1")
    let make = ""
    let matang = ""
    let mao = ""

    if (value == 2) {
        makhuvuc = $(".page1_day").val()
        make = $(".page1_make").val()
        matang = $(".page1_matang").val()
        mao = $(".page1_mao").val()
        CBMTongPage = CBMConLaiPage1
    }
    else {
        makhuvuc = $(".page2_day").val()
        make = $(".page2_make").val()
        matang = $(".page2_matang").val()
        mao = $(".page2_mao").val()
        CBMTongPage = CBMConLaiPage2
    }

    $(".list_dsThung .gopitem").each(function () {
        let $div = $(this);
        let magop = $(".madh option:selected").data("magop")
        if (!$div.find(".checkItemDSThung").prop("checked")) return
        let $trStt = $div.data("stthung")
        let $trView = $div.data("viewthung")
        let pkl = $div.data("mapkl")
        let mahang = $div.data("mahang")
        let colorid = $div.data("colorid")
        let dausize = $div.data("dausizeid")
        let poid = $div.data("poid")
        let po = $div.data("po")
        let madh = $div.data("madh")
        let size = $div.data("size")
        let ds = $div.data("ds")
        let color = $div.data("color")
        let nameSize = $div.data("namesize")
        let SoLuong = $div.data("soluong")
        let $divKL = parseFloat($div.data("khoiluong"));
        $tongCBM += $divKL;
        let object = {
            MaDH: madh,
            MaHang: mahang,
            Ma_PKL: pkl,
            MaGop: magop,
            MaDVSX: madvsx,
            SizeID: size,
            Size: nameSize,
            PoID: poid,
            Po: po,
            DauSizeID: dausize,
            DauSize: ds,
            ColorID: colorid,
            Color: color,
            MaKhuVuc: makhuvuc,
            MaKho: makho,
            MaKe: make,
            MaTang: matang,
            MaO: mao,
            NVNhap: user,
            SttThung: $trStt,
            SttThungView: $trView,
            KhoiLuong: $divKL,
            SoLuong: SoLuong
        };
        arrThis.push(object)
    });
    $tongCBM = Math.round($tongCBM * 10000) / 10000;
    let classNameRomove = value == 1 ? `checkBoxlistDi  ` : `checkBoxlistDen `
    let classNameAdd = value == 2 ? `checkBoxlistDi  ` : `checkBoxlistDen `
    let datacbmDen = value == 1 ? $(".CBMPage1").find(".page_cbmcl").text() : $(".CBMPage2").find(".page_cbmcl").text()
    let datacbmDi = value == 2 ? $(".CBMPage1").find(".page_cbmcl").text() : $(".CBMPage2").find(".page_cbmcl").text()
    let classCbmTru = value == 2 ? "CBMPage1" : "CBMPage2"
    let classCbmCong = value == 1 ? "CBMPage1" : "CBMPage2"
    console.log(classCbmTru)
    console.log(classCbmCong)
    if (!$(".changeo").hasClass("active")) {
        $(`.${classNameRomove} .checkBoxItem`).each(function () {
            let $div = $(this);
            let magop = $div.data("magop")
            if (!$div.find("input").prop("checked")) return;
            let $trStt = $div.data("stthung");
            let $trView = $div.data("viewthung");
            let $divKL = parseFloat($div.data("khoiluong"));
            let pkl = $div.data("mapkl")
            let mahang = $div.data("mahang")
            let colorid = $div.data("colorid")
            let dausize = $div.data("dausizeid")
            let poid = $div.data("poid")
            let madh = $div.data("madh")
            let size = $div.data("size")
            let ds = $div.data("ds")
            let color = $div.data("color")
            let nameSize = $div.data("namesize")
            let po = $div.data("po")
            let SoLuong= $div.data("soluong")
            $tongCBMTD += $divKL;
            let object = {
                MaDH: madh,
                MaHang: mahang,
                MaGop: magop,
                Ma_PKL: pkl,
                MaDVSX: madvsx,
                SizeID: size,
                Size: nameSize,
                PoID: poid,
                Po: po,
                DauSizeID: dausize,
                DauSize: ds,
                ColorID: colorid,
                Color: color,
                MaKhuVuc: makhuvuc,
                MaKho: makho,
                MaKe: make,
                MaTang: matang,
                MaO: mao,
                NVNhap: user,
                SttThung: $trStt,
                SttThungView: $trView,
                KhoiLuong: $divKL,
                SoLuong: SoLuong
            };
            arrThis.push(object);

        })
    }
    $tongCBM += $tongCBMTD
    if (arrThis.length == 0) return;
    if ($tongCBM > datacbmDi) {
        toastr.error("Các thùng có CBM lớn hơn CBM của Ô !")
        return;
    }
    let cbmDi = Number(datacbmDi) - $tongCBM
    cbmDi = Math.round(cbmDi * 10000) / 10000;
    let cbmden = $tongCBMTD + Number(datacbmDen)
    let cbmCong = Math.round(cbmden * 10000) / 10000;
    $(`.${classCbmTru}`).find(".page_cbmcl").text(cbmDi)
    $(`.${classCbmTru}`).attr("data-cbm", cbmDi)
    $(`.${classCbmCong}`).find(".page_cbmcl").text(cbmCong)
    $(`.${classCbmCong}`).attr("data-cbm", cbmCong)
    renderPagenew(arrThis, classNameAdd)
    $(`.${classNameRomove} .checkBoxItem`).each(function () {
        let $div = $(this);
        if (!$div.find("input").prop("checked")) return;
        $div.remove()

    })
    postViTriKho(arrThis)
    //$tongCBM += $tongCBMTD
    //let CBMConLai = $thisDen.data("khoiluong")

    //if (valitem == 1) {
    //    if ((CBMTongPage - $tongCBM) < 0) {
    //        toastr.error("Vui lòng CBM các thùng chọn nhỏ hơn CBM của ô")
    //        return;
    //    } else {
    //        if ($thisNew != "") {
    //            $thisNew.each(function () {
    //                let $div = $(this);
    //                if (!$div.find("input").prop("checked")) return;
    //                $div.remove()
    //            })

    //        }
    //        renderPagenew(arrThis, $thisAdd)
    //    }
    //}
    //else {
    //    if ((CBMTongPage - $tongCBM) < 0) {
    //        return;
    //    } else {
    //        if ($thisNew != "") {
    //            $thisNew.each(function () {
    //                let $div = $(this);
    //                if (!$div.find("input").prop("checked")) return;
    //                $div.remove()
    //            })
    //        }
    //        renderPasteTD($thisAdd, arrThis)
    //    }

    //}
}

function renderPagenew(data, classNameRomove) {
    let html = '';
    let checkHide = ($(".eye_viewXem").hasClass("fa-eye")) ? "hidden" : ""
    data.forEach((item, index) => {
        html += `
            <div class="checkBoxItem" data-soluong="${item.SoLuong}"  data-po="${item.Po}" data-namesize="${item.Size}" data-ds="${item.DauSize}" data-color="${item.Color}" data-poid="${item.PoID}" data-dausizeid="${item.DauSizeID}" data-colorid="${item.ColorID}" data-mapkl="${item.Ma_PKL}" data-mahang="${item.MaHang}"
                data-stthung="${item.SttThung}" data-viewthung="${item.SttThungView}" data-magop="${item.MaGop}" data-khoiluong='${item.KhoiLuong}' data-madh="${item.MaDH}" data-size="${item.SizeID}"">
                                <p>${item.SttThungView}</p>
                                <input class="sttthung${item.SttThung}${item.MaGop}${item.Po}${item.Ma_PKL}" type="checkbox" ${checkHide}/>
                            </div>
            `

    });
    $(`.${classNameRomove}`).append(html)

    Swal.fire({
        title: 'Chuyển thùng thành công!',
        icon: 'success',
        showConfirmButton: false,
        timer: 1500, // Thời gian hiển thị thông báo (1500 ms = 1.5 giây)
        timerProgressBar: true, // Hiển thị thanh tiến trình của timer
        allowOutsideClick: false,
        backdrop: true,
        didOpen: () => {
            Swal.showLoading(); // Hiển thị thanh tiến trình khi thông báo đang được hiển thị
        }
    });
    $(".item_checkpage1").prop("checked", false)
    $(".item_checkpage2").prop("checked", false)
}
function sumCBMPage() {
    let cbmPage1 = 0
    let cbmPage2 = 0
    $(".checkBoxlistDi").find(".checkBoxItem").each(function () {
        let $div = $(this);
        let $divKL = parseFloat($div.data("khoiluong"));
        cbmPage1 += $divKL
    })
    let cbmtongPage1 = Number(CBMTotalPage1) - cbmPage1;
    cbmtongPage1 = Math.round(cbmtongPage1 * 1000) / 1000;
    $(".CBMPage1").text(`(${cbmtongPage1}/${CBMTotalPage1})`)
    CBMConLaiPage1 = cbmtongPage1
    $(".checkBoxlistDen").find(".checkBoxItem").each(function () {
        let $div = $(this);
        let $divKL = parseFloat($div.data("khoiluong"));
        cbmPage2 += $divKL
    })
    let cbmtongPage2 = Number(CBMTotalPage2) - cbmPage2;
    cbmtongPage2 = Math.round(cbmtongPage2 * 1000) / 1000;
    $(".CBMPage2").text(`(${cbmtongPage2}/${CBMTotalPage2})`)
    CBMConLaiPage2 = cbmtongPage2
}
function chaneSttThung() {
    eachThisDi()
    eachThisDen()
}

function changeViTriPage1(makePage1, tangPage1, oPage1) {
    setValueAndTrigger2(".page2_make", makePage1)
        .then(() => setValueAndTrigger2(".page2_matang", tangPage1))
        .then(() => setValueAndTrigger2(".page2_mao", oPage1))
}
function changeViTriPage2(makePage2, tangPage2, oPage2, dataTenKhuVuc) {
    setValueAndTrigger2(".page1_day", dataTenKhuVuc)
        .then(() => setValueAndTrigger2(".page1_make", makePage2))
        .then(() => setValueAndTrigger2(".page1_matang", tangPage2))
        .then(() => setValueAndTrigger2(".page1_mao", oPage2))
}
function setValueAndTrigger2(selector, value) {
    return new Promise((resolve) => {
        $(selector).val(value).trigger("change");
        setTimeout(() => resolve(), 300);
    });
}

function renderValueNew() {
    if (indexvitri == 1 || indexvitri == 2) {
        $(".page2").trigger("click");
        if (indexvitri == 2) {
            changeViTriPage1(dataKe, dataTang, dataMaO)
        } else {
            changeViTriPage2(dataKe, dataTang, dataMaO, dataTenKhuVuc)
        }
    }
    flagCheckNew = false
}
$(".chuyenList").on("click", "i", function () {
    var button = $(this);
    button.addClass('clicked');

    setTimeout(function () {
        button.removeClass('clicked');
    }, 500);
})
let flagCheckBoxPage = 0
$(".item_checkpage1").on("change", function () {
    let $thisDivNew = $(".checkBoxlistDi")
    if ($thisDivNew.find(".checkBoxItem").length > 0) {
        if ($(this).prop("checked")) {
            showConfirmationCheck($thisDivNew)
            flagCheckBoxPage = 1
        }
        else {
            showConfirmationUnCheck($thisDivNew)
            flagCheckBoxPage = 1
        }
    }

})
$(".item_checkpage2").on("change", function () {
    let $thisDivNew = $(".checkBoxlistDen")
    if ($thisDivNew.find(".checkBoxItem").length > 0) {
        if ($(this).prop("checked")) {
            showConfirmationCheck($thisDivNew)
            flagCheckBoxPage = 2
        }
        else {
            showConfirmationUnCheck($thisDivNew)
            flagCheckBoxPage = 2
        }
    }
})
let $divTuThung = ""
let $divDenThung = ""
let $thisCheckAllTD = ""
let checkflag = ""
function showConfirmationCheck($this) {
    checkflag = true
    uncheckBox = false
    Swal.fire({
        title: 'Thực hiện hành vi?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Chọn thùng',
        cancelButtonText: 'Tất cả thùng',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        showCloseButton: true,// Thêm nút đóng
        html: '<button id="clear1" class="swal2-confirm swal2-styled" style="background-color: #f6c23e;">Hủy</button>'

    }).then((result) => {
        if (result.isConfirmed) {
            let $thisCheckAllTD = $this.find(".checkBoxItem")
            let colorMap = {};
            let arrCheckOther = []
            $thisCheckAllTD.each(function () {
                let $thisItem = $(this);
                let po = $thisItem.data("poid");
                let dausize = $thisItem.data("dausizeid");
                let pkl = $thisItem.data("mapkl");
                let color = $thisItem.data("colorid");
                let donhang = $thisItem.data("madh");
                let size = $thisItem.data("size");
                let ds = $thisItem.data("ds")
                let namecolor = $thisItem.data("color")
                let nameSize = $thisItem.data("namesize")
                let namepo = $thisItem.data("po")
                let sttthung = $thisItem.data("stthung")
                let gopitem = `${po}${dausize}${pkl}${color}${donhang}${size}`;
                let gopitemnew = `MaDH: ${donhang} - PO: ${namepo} - ${pkl} - Size: ${nameSize} `
                let object =
                {
                    MaDH: donhang,
                    PO: po,
                    MaPKL: pkl,
                    Color: color,
                    Size: size,
                    DauSize: dausize,
                    GopItem: gopitemnew
                }
                if (!colorMap[gopitem]) {
                    colorMap[gopitem] = true;
                    arrCheckOther.push(object);
                }

            });
            popupTheoViTri(arrCheckOther)
        }
    });
    $(".swal2-cancel").on("click", function () {
        checkAllThungTD($this, true)
    })
    $("#clear1").on("click", function () {
        if (flagCheckBoxPage == 1) {
            $(".item_checkpage1").prop("checked", false)
        }
        else $(".item_checkpage2").prop("checked", false)
        Swal.close();
    })
    $(".swal2-close").on("click", function () {
        if (flagCheckBoxPage == 1) {
            $(".item_checkpage1").prop("checked", false)
        }
        else $(".item_checkpage2").prop("checked", false)
        Swal.close();
    })

}
let uncheckBox = false
function showConfirmationUnCheck($this) {
    checkflag = false
    uncheckBox = false
    Swal.fire({
        title: 'Thực hiện hành vi?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Hủy thùng',
        cancelButtonText: 'Hủy tất cả thùng',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        showCloseButton: true,// Thêm nút đóng
        html: '<button id="clear1" class="swal2-confirm swal2-styled" style="background-color: #f6c23e;">Hủy</button>'

    }).then((result) => {
        if (result.isConfirmed) {
            let $thisCheckAllTD = $this.find(".checkBoxItem")
            let colorMap = {};
            let arrCheckOther = []
            $thisCheckAllTD.each(function () {
                let $thisItem = $(this);
                let po = $thisItem.data("poid");
                let dausize = $thisItem.data("dausizeid");
                let pkl = $thisItem.data("mapkl");
                let color = $thisItem.data("colorid");
                let donhang = $thisItem.data("madh");
                let size = $thisItem.data("size");
                let ds = $thisItem.data("ds")
                let namecolor = $thisItem.data("color")
                let nameSize = $thisItem.data("namesize")
                let namepo = $thisItem.data("po")
                let sttthung = $thisItem.data("stthung")
                let gopitem = `${po}${dausize}${pkl}${color}${donhang}${size}`;
                let gopitemnew = `MaDH: ${donhang} - PO: ${namepo} - ${pkl} - Size: ${nameSize} `
                let object =
                {
                    MaDH: donhang,
                    PO: po,
                    MaPKL: pkl,
                    Color: color,
                    Size: size,
                    DauSize: dausize,
                    GopItem: gopitemnew
                }
                if (!colorMap[gopitem]) {
                    colorMap[gopitem] = true;
                    arrCheckOther.push(object);
                }

            });
            popupTheoViTri(arrCheckOther)
            uncheckBox = true
        }
    });
    $(".swal2-cancel").on("click", function () {

        checkAllThungTD($this, false)
    })
    $("#clear1").on("click", function () {
        if (flagCheckBoxPage == 1) {
            $(".item_checkpage1").prop("checked", false)
        }
        else $(".item_checkpage2").prop("checked", false)
        Swal.close();
    })
    $(".swal2-close").on("click", function () {
        if (flagCheckBoxPage == 1) {
            $(".item_checkpage1").prop("checked", false)
        }
        else $(".item_checkpage2").prop("checked", false)
        Swal.close();
    })

}
$(".popup").on("click", ".save_list", function () {
    let valueDenthung = $(".item_denthung").val()
    let valueTuThung = $(".item_tuthung").val()
    if (valueDenthung == "" || valueTuThung == "") return
    eachTDGopItem(valueTuThung, valueDenthung)
    $(".popup").hide()
})
$(".popup").on("click", ".cancel_list", function () {
    $(".item_denthung").val("")
    $(".item_tuthung").val("")
    $(".popup").hide()
    if (flagCheckBoxPage == 1) {
        $(".item_checkpage1").prop("checked", uncheckBox)
    }
    else $(".item_checkpage2").prop("checked", uncheckBox)
})

function eachTDGopItem(tuthung, denthung) {
    let MaDH = $(".totalLoc").val()
    let PO = $(".totalLoc option:selected").data("po")
    let Size = $(".totalLoc option:selected").data("size")
    let MaPKL = $(".totalLoc option:selected").data("mapkl")
    let $thisDiv = ""
    if (flagCheckBoxPage == 1) {
        $thisDiv = $(".checkBoxlistDi .checkBoxItem")
    }
    else $thisDiv = $(".checkBoxlistDen .checkBoxItem")
    $thisDiv.each(function () {
        let $div = $(this);
        let po = $div.data("poid");
        let pkl = $div.data("mapkl");
        let donhang = $div.data("madh");
        let size = $div.data("size");
        let $trView = $div.data("viewthung")
        if ($trView >= Number(tuthung) && $trView <= Number(denthung) && MaDH == donhang && PO == po && Size == size && MaPKL == pkl) {
            $div.find("input").prop("checked", checkflag)
            let $class = $div.find("input").attr("class")
            $(".rendertable_list").find(`.${$class}`).prop("checked", checkflag)
        }

    })
}
function checkAllThungTD($this, vlauecheck) {
    let $td = $this.find(".checkBoxItem")
    $td.find("input").prop("checked", vlauecheck)
    $td.each(function () {
        let $class = $(this).find("input").attr("class")
        $(".rendertable_list").find(`.${$class}`).prop("checked", vlauecheck)
    })
}

function popupTheoViTri(arrCheckOther) {
    let htmlLoc = ``
    arrCheckOther.map(item => {
        htmlLoc += `
                        <option data-po="${item.PO}" data-mapkl="${item.MaPKL}" data-color="${item.Color}" data-size="${item.Size}" value="${item.MaDH}">${item.GopItem}</option>
                `

    })
    $(".totalLoc").html(htmlLoc)
    vitriItem()
}
$(".totalLoc").on("change", function () {
    vitriItem()
})
function vitriItem() {
    let arr = []
    let MaDH = $(".totalLoc").val()
    let PO = $(".totalLoc option:selected").data("po")
    let Size = $(".totalLoc option:selected").data("size")
    let MaPKL = $(".totalLoc option:selected").data("mapkl")
    let $thisDiv = ""
    if (flagCheckBoxPage == 1) {
        $thisDiv = $(".checkBoxlistDi .checkBoxItem")
    }
    else $thisDiv = $(".checkBoxlistDen .checkBoxItem")
    $thisDiv.each(function () {
        let $thisItem = $(this);
        let po = $thisItem.data("poid");
        let pkl = $thisItem.data("mapkl");
        let donhang = $thisItem.data("madh");
        let size = $thisItem.data("size");
        let nameThung = $thisItem.data("viewthung")
        if (MaDH == donhang && PO == po && Size == size && MaPKL == pkl) {
            arr.push(nameThung)
        }
    })
    arr.sort(function (a, b) {
        return a - b;
    });
    let smallest = arr[0];
    let largest = arr[arr.length - 1];
    $divTuThung = smallest
    $divDenThung = largest
    let khoanvitri = ``
    $(".item_tuthung").val("")
    $(".item_denthung").val("")
    $(".popup").show()

    let html = `
                    <div class="list_SizeColor">
                        <div class="item_size">
                            <p>Khoản vị trí: <span> ${khoanvitri} </span></p>
                        </div>
                  
    `
    $(".display").html(html)
}
$(".chuyenList").on("click", ".eye_viewXem", function () {
    $(this).toggleClass('fa-eye-slash fa-eye');
    if ($(this).hasClass("fa-eye"))
        $(".checkBoxlist").find("input").hide()
    else
        $(".checkBoxlist").find("input").show()
})
$(".checkBoxlist ").on("click", ".checkBoxItem", function () {
    if ($(".eye_viewXem").hasClass("fa-eye")) {
        popupDetailThung($(this))
    }
})
$('.clickActive').on('select2:open', function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))
        $('body').addClass('active5');
});
// Khi input tìm kiếm của Select2 mất focus
$('.clickActive').on('select2:close', function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))
        $('body').removeClass('active5');
});
$(".changeo").on("click", function () {
    $(this).toggleClass("active")
    if ($(this).hasClass("active")) {
        $(".boxDen").addClass("active")
        $(this).find("p").text("Chuyển ô")
    }
    else {
        $(".boxDen").removeClass("active")
        $(this).find("p").text("Ẩn chuyển ô")
    }
})
$(".page1_make", ".page2_make", ".page1_matang", ".page2_matang", ".page1_mao", ".page2_mao", ".page1_day", ".page2_day").select2()
function rederPageDay() {
    let dvsx = $(".nhamay").val()
    let tenkho = $(".tenkho").val()
    let tenkhuvuc = $(".tenkhuvuc").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetDay&Para1=${dvsx}&Para2=${tenkho}&Para3=${tenkhuvuc}&Para4=a&Para5=a&Para6=a&Para7=$a`,
        type: "Get",
        success: function (data) {
            let html = ` <option></option>`
            data.map(item => {
                html += `
                        <option  value="${item.MaDay}">${item.NameDay}</option>
                `
            })
            $(".page1_day").html(html)
            $(".page2_day").html(html)
            rederPageKe1()
            rederPageKe2()
        }

    })
}
$(".page1_day").change(rederPageKe1)
$(".page2_day").change(rederPageKe2)