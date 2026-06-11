

toastr.options = {
    "closeButton": true,
    "debug": false,
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
};
let flagCheckNew = true;
$(function () {
    renderNhaMay()
    $(".nhamay").on("change", function () {
        renderTenKho()
    })
    $(".tenkho").on("change", function () {
        renderTenKhuVuc()
    })
    $(".tenkhuvuc").on("change", function () {
        renderKe()
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
function renderTenKhuVuc(value) {
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
                renderKe(value)

            }
        }
    })
}

async function renderKe(value) {
    try {
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
                    let valueo = `<p class="nameo" style="margin-top:2px;white-space: nowrap;margin-left: 5px;">${o.NameO}</p>`
                    let chuyenIndex = `<i class="fa-solid fa-arrow-right arrowIndex"></i>`

                    let checkAll = `  <label class="container">
                      <input class="inputcheckmark" type="checkbox">
                      <span class="checkmark"></span>
                    </label>`
                    let inputName = `
                        <input type="number" name="" class="inputSL active" placeholder="Nhập SL">
                    `
                    let cbmO = Math.round(o.CBMTotal * 1000) / 1000
                    let iconPast = `<i class="fa-duotone fa-solid fa-paste pasteIcon"></i>`
                    iconmau = `
                             ${valueo} ${chuyenIndex}  <div class="list_p"> ${eye} ${inputName}  ${checkAll} <p class="CBMTD">  (${cbmO}/${o.CBM})</p></div>`;

                    let htmlnew = ""
                    let htmlnew2 = ""
                    if (cbmO != o.CBM) {
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
                        <div class="item_thung1"  >
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
        removeLoading()
        if (valueCBM != 0) {
            getGoiY(valueCBM)
        }
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
        let htmlnew2 = await renderThungInKeChiTiet(make, matang, mao)
        $thisTD.find(".item_thung1").html(htmlnew2)
    })
}
$(".rendertable_list").on("change", ".inputcheckmark", function () {
    let $this = $(this)
    if ($this.prop("checked")) {
        $this.closest("td").find(".inputSL").removeClass("active")
    }
    else {
        $this.closest("td").find(".inputSL").addClass("active")
    }
})
$(".rendertable_list").on("input", ".inputSL", function () {
    let $this = $(this)
    let value = $this.val()
    if (slThungKienCL < value) {
        toastr.error('Vui lòng không nhập quá số kiện còn lại.', 'Error');
        $this.val(value.slice(0, -1));
    }
})
$(".rendertable_list").on("click", ".arrowIndex", function () {
    let $this = $(this)
    let $thisTD = $this.closest("td")
    let slKien = $thisTD.find(".inputSL").val()
    let make = $thisTD.data("make")
    let matang = $thisTD.data("tang")
    let mao = $thisTD.data("o")
    let cbmconlai = $thisTD.data("cbmcl")
    if (slKien == 0) {
        toastr.warning('Vui lòng nhập số kiện để chuyển vào ô.', 'Warning');
    }
    console.log(cbmconlai)
    saveListKien(slKien, make, matang, mao, cbmconlai, $thisTD)
})

let resultKV = []
async function getGoiY(value) {
    let dvsx = $(".nhamay").val()
    let makho = $(".tenkho").val()
    const [response1] = await Promise.all([
        fetch(`/api/ViTriKhoTong/Get?Action=GetGoiY&Para1=${dvsx}&Para2=${makho}&Para3=a&Para4=a&Para5=a&Para6=a&Para7=a&Para8=a&Para9=a`),
    ]);
    let data = await response1.json()
    if (data.length == 0) return;
    let arrData = []
    let sumCBM = 0;
    if (value == 0) {
        $(".goiythung").text("Số lượng kiện hoặc CBM = 0 ! Không gợi ý được vị trí ô")
        return;
    }
    let arrO = []
    let arrKhuVuc = []
    for (var i = 0; i < data.length; i++) {
        sumCBM += data[i].CBM
        if (data[i].CBM - data[i].CBMDetail > 1) {
            arrO.push(data[i].NameO)
            let object = {
                MaKhuVuc: data[i].MaDay,
                TenKhuVuc: data[i].TenDay
            }
            arrKhuVuc.push(object)
        }
        if (sumCBM > value) {
            break;
        }
    }
    let goiyo = arrO.join(", ")
    resultKV = arrKhuVuc.reduce((unique, item) => {
        return unique.some(t => t.MaKhuVuc === item.MaKhuVuc) ? unique : [...unique, item];
    }, []);
    $(".goiythung").html(`Đơn hàng này có thể đưa vào các vị trí ô:<span class="vitrio" style="color: #8d8b23;"> ${goiyo}</span>`)
}

function checkVitri() {
    if (valueCBM == 0) return;
    flagCheckNew = false;
    let arrKVNew = resultKV
    filterO(arrKVNew)
}
function filterO(arrKVNew) {
    if (arrKVNew.length > 1 || $(".tenkhuvuc").val() != arrKVNew[0].MaKhuVuc) {
        $('#myModal').modal('show');
        let html = ``
        arrKVNew.map(item => {
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
$(".modal-body").on("click", '.item_kv', function () {
    changekv($(this).attr("id"))
})
let checkgoiychange = true
function changekv(id) {
    let valuekv = $(".tenkhuvuc").val()
    if (valuekv == id) {
        goiyino()
    }
    else {
        checkgoiychange = false
        $(".tenkhuvuc").val(id).trigger("change")
    }
    $('#myModal').modal('hide');
}
function goiyino() {
    if ($(".tenkho").val() == undefined) {
        toastr.warning('Vui lòng chọn tên kho', 'Warning');
        return;
    }
    let vitrio = $(".goiythung").find(".vitrio").text()
    let result = vitrio.split(",").map(item => item.trim())
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
$(".iconNapLap").on("click", function () {
    flagCheckNew = true;
    $(".rendertable_list").find("td").not(".rowMerge").show();
})
$(".goiy").click(function () {
    $(this).toggleClass("active2")
    if (!$(this).hasClass("active2")) {
        $(this).find("span").text("Gợi ý")
        flagCheckNew = true;
        $(".rendertable_list").find("td").not(".rowMerge").show();

    } else {
        $(this).find("span").text("Hủy gợi ý")
        checkVitri()
    }

})
async function saveListKien(slKien, make, matang, mao, cbmconlai, $thisTD) {
    let maDH = $(".madh").val()
    let PO = $(".PO").val()
    let POText = $(".PO option:selected").text()
    let pkl = $(".maPKL").val()
    let dot = $(".dot").val()
    let dvsx = $(".nhamay").val()
    let makho = $(".tenkho").val()
    let makhuvuc = $(".tenkhuvuc").val()
    let dataSoKienCL = "";
    let dataSokien = "";

    const [response1, response2] = await Promise.all([
        fetch(`/api/ViTriKhoTong/Get?Action=GetThungDaNhap&Para1=${pkl}&Para2=${PO}&Para3=${maDH}&Para4=a&Para5=a&Para6=a&Para7=a&Para8=a&Para9=a`),
        fetch(`/api/ViTriKhoTong/Get?Action=GetKien&Para1=${pkl}&Para2=${PO}&Para3=${maDH}&Para4=${dot}&Para5=a&Para6=a&Para7=a&Para8=a&Para9=a`)
    ]);

    dataSoKienCL = await response1.json();
    dataSokien = await response2.json();
    dataSokien = dataSokien.filter(x => !dataSoKienCL.some(x1 => x1.SttThung === x.SttThung))
    let cbmo = cbmconlai;
    let cbmSttThung = 0;
    let arrThung = []
    let check = true;
    if (dataSokien[0].SttThung_decat != 0) {
        check = false;
    }

    for (var i = 0; i < slKien; i++) {
        let item = dataSokien[i]
        cbmSttThung += item.KhoiLuong
        if (cbmSttThung > cbmo) {
            if (i == 0) {
                toastr.warning('Ô đã hết cbm vui lòng chọn ô khác!', 'Warning');
                break;
            }
            Swal.fire({
                title: `<div><p>Ô hiện tại chỉ chứa được  ${i} thùng!</p><p> Bạn có nhập vào ô không ?</p></div>`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                cancelButtonText: 'Hủy',
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                showCloseButton: true,// Thêm nút đóng
            }).then(async (result) => {
                if (result.isConfirmed) {
                    let cbmtong = cbmo - (cbmSttThung - item.KhoiLuong)
                    cbmtong = Math.round(cbmtong * 1000) / 1000;
                    await saveKien(arrThung)
                    let htmlnew2 = await renderThungInKeChiTiet(make, matang, mao)
                    $thisTD.find(".item_thung1").html(htmlnew2)
                    $thisTD.find(".inputSL").val("")
                    $thisTD.find(".CBMTD").text(`(${cbmtong}/${cbmo})`)
                    $thisTD.data('cbmcl', cbmtong);
                }

            });
            break;
        }
        let sttView = check == true ? item.SttThung_temp : item.SttThung_decat
        let sttthung = item.SttThung
        let object = {
            MaDH: maDH,
            MaHang: item.MaHang,
            Ma_PKL: pkl,
            MaDVSX: dvsx,
            SizeID: item.SizeID,
            Size: item.Size,
            PoID: PO,
            Po: POText,
            DauSizeID: item.DauSizeID,
            DauSize: item.DauSize,
            ColorID: item.ColorID,
            Color: item.TenMau,
            MaKhuVuc: makhuvuc,
            MaKho: makho,
            MaKe: make,
            MaTang: matang,
            MaO: mao,
            NVNhap: user,
            SttThung: sttthung,
            SttThungView: sttView,
            KhoiLuong: item.KhoiLuong
        };
        arrThung.push(object)
    }
    let cbmtong = cbmo - cbmSttThung;
    cbmtong = Math.round(cbmtong * 1000) / 1000;

    if (!Swal.isVisible() && arrThung.length > 0) {
        await saveKien(arrThung)
        let htmlnew2 = await renderThungInKeChiTiet(make, matang, mao)
        $thisTD.find(".item_thung1").html(htmlnew2)
        $thisTD.find(".CBMTD").text(`(${cbmtong}/${cbmo})`)
        $thisTD.data('cbmcl', cbmtong);
        $thisTD.find(".inputSL").val("")
    }


}
async function saveKien(arrThung) {
    $.ajax({
        type: "POST",
        url: "/api/ViTriKhoTong/Post",
        data: JSON.stringify(arrThung),
        contentType: 'application/json; charset=utf-8',
        success: async function (data) {
            await rederSThung(1)
            toastr.success('Lưu thành công', 'Success');

        }
    });


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
    renderDot()
}
$(".maPKL").on("change", function () {
    renderDot()
})
let slThungKienCL = 0;
let valueCBM = 0;
function rederSThung(value) {

    let maDH = $(".madh").val()
    let PO = $(".PO").val()
    let pkl = $(".maPKL").val()
    let dot = $(".dot").val()

    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetMaSoKien&Para1=${pkl}&Para2=${PO}&Para3=${maDH}&Para4=${dot}&Para5=a&Para6=a&Para7=a&Para8=a&Para9=a`,
        type: "Get",
        success: function (data) {
            if (data.length == 0) {
                $(".view-thung").empty()
                return;
            }
            let d = data[0]
            slThungKienCL = d.SLTConLai
            let html = `
                 <div>
                          <i style="color: #35c2c3;" class="fa-sharp-duotone fa-solid fa-circle"></i>
                          Số lượng thùng:<span style="font-weight: bold; font-family: 'Roboto'; margin-left: 5px;">${d.SoThung} (kiện)</span>
                      </div>
                      <div>
                          <i style="color: #64c335;" class="fa-sharp-duotone fa-solid fa-circle"></i>
                          CBM tổng: <span style="font-weight: bold; font-family: 'Roboto'; margin-left: 5px;">${d.KhoiLuong} (CBM)</span>
                      </div>
                      <div>
                          <i style="color: #209d93;" class="fa-sharp-duotone fa-solid fa-circle"></i>
                          Số lượng thùng CL: <span style="font-weight: bold; font-family: 'Roboto'; margin-left: 5px;">${d.SLTConLai} (kiện)</span>
                      </div>
                      <div>
                          <i style="color: #3552c3; " class="fa-sharp-duotone fa-solid fa-circle"></i>
                          CBM tổng CL:<span style="font-weight: bold; font-family: 'Roboto'; margin-left: 5px;">${d.CBMconlai} (CBM)</span>
                      </div>
            
            `
            $(".view-thung")
            $(".view-thung").html(html)
            if (value != 1)
                getGoiY(d.CBMconlai)
            valueCBM = d.CBMconlai
        }

    })
}


function renderDot() {
    let maDH = $(".madh").val()
    let PO = $(".PO").val()
    let pkl = $(".maPKL").val()
    $.ajax({
        url: `/api/ViTriKhoTong/Get?Action=GetDot&Para1=${pkl}&Para2=${PO}&Para3=${maDH}&Para4=a&Para5=a&Para6=a&Para7=a&Para8=a&Para9=a`,
        type: "Get",
        success: function (data) {
            let html = `<option value="All">Tất cả</option>`
            data.map(item => {
                html += `
                    <option value="${item.DotDT}">${item.DotDT}</option>
                    `
            })
            $(".dot").html(html)
            rederSThung()
        }
    })
}
$('.rendertable_list').on('focus', ".inputSL", function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $('body').addClass('active5');
    }
});
$('.rendertable_list').on('blur', ".inputSL", function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $('body').removeClass('active5');
    }
});
$(".btnChiTietGiaCong").on("click", function () {
    const dvsx = $(".nhamay").val()
    const tenkho = $(".tenkho").val()
    const tenkhuvuc = $(".tenkhuvuc").val()
    const indexViTri = "0"
    window.location.href = `/SoDoKho/SoDo?dvsx=${dvsx}&tenkho=${tenkho}&tenkhuvuc=${tenkhuvuc}&indexViTri=${indexViTri}&username=${user}`;
})

$(".madh,.PO,.maPKL,.dot").select2()