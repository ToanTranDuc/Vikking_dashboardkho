var userNameSave = localStorage.getItem("username1")

var kiengocAttr = ""
var soluonggoc = ""
var barCodeGoc = ""
var trSolo = ""
var arrXH = []
var $thisTrDelete;
var manplQuet = ""
var soloidQuet = ""
var maphieuyc = ""
var ngayNK = ""
var SoMet = 0
const tdFields = ['SoLo', 'ChiTiet', 'MauVT', 'SoKien', 'ThucNhap', 'SoLoT', 'KhoVai', 'TenDVVT', 'GhiChu'];
$(function () {
    $(".select_2").select2();

    GetMaHang();
    GetLo();
    $("#Lo_Select").on("click", function () {
        showHidePopTablePD()
    })
    $(".chiakien").on("click", function () {
        $('#myModal').modal('show');
    })
    $("#btnSaveTachKien").on("click", async function () {

        await tachKien();
    })

    document.getElementById('modalSoLuong').addEventListener('input', function (e) {

        var thucnhap = $("#modalThucNhap").text().trim();
        var sokientach = document.getElementById('modalSoKienTach').value;
        var slmax = (parseFloat(thucnhap) / sokientach).toFixed(2);
        var soLuongInput = document.getElementById('modalSoLuong');

        let value = soLuongInput.value;


        value = value.replace(/[^0-9.]/g, '');


        let parts = value.split('.');
        if (parts.length > 2) {
            value = parts[0] + '.' + parts.slice(1).join('');
        }

        if (value.indexOf('.') >= 0) {
            let [intPart, decimalPart] = value.split('.');
            decimalPart = decimalPart.slice(0, 2);
            value = intPart + '.' + decimalPart;
        }

        if (Number(value) > slmax) {
            //alert('Số lượng nhập không được vượt quá ' + slmax);
            iziToast.warning({
                title: `Warning`,
                message: `Số lượng nhập không đc vượt quá ${slmax}`,
                position: 'topRight'
            });
            soLuongInput.value = value.slice(0, -1);
            return;
        }

        soLuongInput.value = value;
    });
    document.getElementById('modalSoKienTach').addEventListener('input', function (e) {
        this.value = this.value.replace(/[^0-9]/g, '');
        if (this.value !== '' && parseInt(this.value, 10) < 1) {

            this.value = '';
        }
    });
    $("#tblDataBody").on("click", ".btnChiaKien", function () {
        if ($("#DH_PKL").val() == "") {

            return
        }
        let trRow = $(this).closest("tr")
        let trKienGoc = trRow.data("kiengoc")
        let trSLGoc = trRow.data("soluonggoc")
        let barCodeGocA = trRow.data("barcodegoc")
        ngayNK = trRow.data("ngaynk")
        kiengocAttr = trKienGoc;
        soluonggoc = trSLGoc
        barCodeGoc = barCodeGocA
        getMaxKien(trKienGoc)
    })

    $(document).on("click", function (event) {
        if (!$(event.target).closest(".popup-tablesearchPD,#Lo_Select").length) {
            $(".popup-tablesearchPD").addClass("active");
        }
    });
    $(".save-buttonShare").on("click", function () {
        Save()
    })
    $("#DH_PKL").on("change", function () {
        GetXuatHang(trSolo)
    })
    $("#tblDataBody").on("click", ".btnXoaKien", function () {
        let $this = $(this).closest("tr")
        $thisTrDelete = $this
        $("#myModalD").modal("show")
    })
    $("#btnDeleteKien").on("click", function () {
        let kiengoctr = $thisTrDelete.data("kiengoc")
        var kiengocnewx = $thisTrDelete.next('tr').data("kiengoc");
        var kienchianext = $thisTrDelete.next('tr').find("td.sokien").text()
        if (kiengoctr == kiengocnewx) {
            iziToast.warning({
                title: `Warning`,
                message: `Vui lòng xóa kiện đã chia : ${kienchianext}`,
                position: 'topRight'
            });
            $("#myModalD").modal("hide")
            return
        }
        else {
            $("#myModalD").modal("hide")
            $thisTrDelete.remove()
            sumKien()

        }
    })
    $("#modalSoKienTach").on("input", function () {
        document.getElementById('modalSoLuong').value = "";
    })
    $("#CayVai_Selected").on("change", function () {
        GetXuatHang(trSolo)
        GetPhieuYeuCau()

    })
    $("#PhieuYeuCau_Selected").on("change", function () {
        SoMet = $("#PhieuYeuCau_Selected option:selected").data("slcl")
    })
    document.querySelector('.btn-clearAll').addEventListener('click', function () {
        var tbody = document.getElementById('tblLo');
        if (!tbody) return;
        var checkboxes = tbody.querySelectorAll('input[type="checkbox"]');
        checkboxes.forEach(cb => cb.checked = false);
        var loSelect = document.getElementById('Lo_Select');
        if (loSelect) loSelect.value = '';
        window.trSolo = '';
        GetCayVai();
    });

    $("#tblDataBody").on("click", ".ipcheckbox", function () {
        CheckRow($(this))
    })
    $(".main-item").on("click", 'input.checkAllXH', function () {
        //if ($("#DH_PKL").val() == "") {
        //    iziToast.warning({
        //        title: `Warning`,
        //        message: `Vui lòng chọn mã hàng`,
        //        position: 'topRight'
        //    });
        //    $(this).prop("checked", false)
        //    return
        //}
        let total = 0;

        $("#tblDataBody tr").each(function () {
            let $row = $(this)
            if ($(".checkAllXH").is(":checked")) {
                if ($(this).find("input.ipcheckbox").is(":checked")) return

                var giaTriThucNhap = parseFloat($row.data("slnhap")).toFixed(2);
                let sometCL = $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucaucong").text().trim()
                let sometC = $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucautru").text().trim()
                let sokien = $row.find("td.sokien").text()

                if (sometCL - giaTriThucNhap < 0) {
                    iziToast.warning({
                        title: 'Warning',
                        message: `Tại kiện ${sokien} đã xuất đủ số mét đã cấp !!!.`,
                        position: 'topRight'
                    });
                    return false
                }
                $row.find("input.ipcheckbox").prop("checked", true)
                $row.find("td.phieuyc").text($("#PhieuYeuCau_Selected").val())

                var sumTfoot = parseFloat(sometCL) - parseFloat(giaTriThucNhap)
                var sumTfootT = parseFloat(sometC) + parseFloat(giaTriThucNhap)
                $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucaucong").text(parseFloat(sumTfoot.toFixed(2)))
                $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucautru").text(parseFloat(sumTfootT.toFixed(2)))
            }

            else {
                if (!$(this).find("input.ipcheckbox").is(":checked")) return

                $(this).find("input.ipcheckbox").prop("checked", false)

                $row.find("td.phieuyc").text("")

                var soluongnhap = $row.data("slnhap")

                $row.find("td.thucnhap").text(soluongnhap)

                let sometCL = $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucaucong").text().trim()
                let sometC = $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucautru").text().trim()

                var sumTfoot = parseFloat(sometCL) + parseFloat(soluongnhap)
                var sumTfootT = parseFloat(sometC) - parseFloat(soluongnhap)
                $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucaucong").text(parseFloat(sumTfoot.toFixed(2)))
                $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucautru").text(parseFloat(sumTfootT.toFixed(2)))

            }
        })
    })
    $(".btn-tab1").on("click", function () {
        $(".tab1").show()
        $(".tab2").hide()
    })
    $(".btn-tab2").on("click", function () {
        $(".tab2").show()
        $(".tab1").hide()
    })
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })
    $(".checkAllBC").on("click", function () {
        $("#tblDataBody tr").each(function () {
            if ($(".checkAllBC").is(":checked")) {
                $(this).find("input.checkbc").prop("checked", true)
            }
            else $(this).find("input.checkbc").prop("checked", false)
        })
    })
    // tab 2 -----------------
    getDate()
    $(".fa-calendar-days").on("click", function () {
        $(".datepicker").focus()
    })
    $(".datepicker").on("change", function () {
        GetPhieuNhapKho()
    })
    $(".icon_reload").on("click", function () {
        GetDataPhieuXuatKho()
    })
})
function CheckRow($this) {
    //if ($("#DH_PKL").val() == "") {
    //    iziToast.warning({
    //        title: `Warning`,
    //        message: `Vui lòng chọn mã hàng`,
    //        position: 'topRight'
    //    });
    //    $this.prop("checked", false)
    //    return
    //}
    var $row = $this.closest("tr");
    var rowClassPYC = $row.find("td.phieuyc").text().trim()
    if (rowClassPYC != "" && rowClassPYC != $("#PhieuYeuCau_Selected").val()) {
        iziToast.warning({
            title: 'Warning',
            message: `Bạn đang chọn phiếu ${$("#PhieuYeuCau_Selected").val()}.Không được sửa kiện đang chọn phiếu ${rowClassPYC} .`,
            position: 'topRight'
        });
        $this.prop("checked", true)
        return
    }

    if ($this.is(":checked")) {

        var giaTriThucNhap = parseFloat($row.data("slnhap")).toFixed(2);
        let sometCL = $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucaucong").text().trim()
        let sometC = $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucautru").text().trim()

        if (sometCL - giaTriThucNhap < 0) {
            $this.prop("checked", false)
            iziToast.warning({
                title: 'Warning',
                message: 'Quá số lượng thực nhập yêu cầu. Không thể xuất hàng!!!.',
                position: 'topRight'
            });
            return
        }
        $row.find("td.phieuyc").text($("#PhieuYeuCau_Selected").val())
        var sumTfoot = parseFloat(sometCL) - parseFloat(giaTriThucNhap)
        var sumTfootT = parseFloat(sometC) + parseFloat(giaTriThucNhap)
        $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucaucong").text(parseFloat(sumTfoot.toFixed(2)))
        $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucautru").text(parseFloat(sumTfootT.toFixed(2)))
    }
    else {
        $row.find("td.phieuyc").text("")
        var soluongnhap = $row.data("slnhap")

        $row.find("td.thucnhap").text(soluongnhap)

        let sometCL = $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucaucong").text().trim()
        let sometC = $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucautru").text().trim()

        var sumTfoot = parseFloat(sometCL) + parseFloat(soluongnhap)
        var sumTfootT = parseFloat(sometC) - parseFloat(soluongnhap)
        $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucaucong").text(parseFloat(sumTfoot.toFixed(2)))
        $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucautru").text(parseFloat(sumTfootT.toFixed(2)))
    }

}
function getDate() {
    $(".datepicker").flatpickr({
        enableTime: false,
        dateFormat: "d-m-Y ",
        time_24hr: false,
        allowInput: true,
        defaultDate: new Date()
    });
    GetPhieuNhapKho()
}
function GetDataBarCode() {
    var mavt = $("#CayVai_Selected option:selected").data("mavt");
    $('.lablebarcode').empty()
    $("#tblDataBody tr").each(function (index) {
        var $trRow = $(this)
        var checkbarCode = $trRow.find("input.checkbc").is(":checked")
        if (!checkbarCode) return
        var chitiet = $trRow.find("td.chitiet").text().trim()
        var mau = $trRow.find("td.mau").text().trim()
        var sokien = $trRow.find("td.sokien").text().trim()
        var thucnhap = $trRow.find("td.thucnhap").text().trim()
        var solot = $trRow.find("td.solot").text().trim()
        var donvi = $trRow.find("td.donvi").text().trim()
        var ghichu = $trRow.find("td.ghichu").text().trim()
        var barcodechia = $trRow.data("barcodechia")
        var ngayNhap = $trRow.data("ngaynk")
        var classGap = index % 2 == 0 ? "pe-`" : "ps-1"
        const labelHTML = `
         <div class="col-6 col-md-6 col-lg-6 d-flex justify-content-center ${classGap} py-2">

            <div class="label-box label-container">
            <div class="label-header">DONAGAMEX</div>
            <div class="content-row">
              <div class="left-col">
                <p class="line-item"> ${mavt}</p>
                <p class="line-item">${chitiet}</p>
                <p class="line-item">${mau}</p>
                <p class="line-item item-gapprint">
                  <span class="inline-group">Kiện: ${sokien}</span>
                  <span class="inline-group">Lót: ${solot}</span>
                  <span class="inline-group">Ghi chú: ${ghichu} </span>
                </p>
                <p class="line-item item-gapprint">
                  <span class="inline-group">Số lượng: ${thucnhap}</span>
                  <span class="inline-group">${donvi}</span>
                </p>
                <p class="line-item">QrCode: <span style="margin-left: 0.5rem;">${barcodechia}</span></p>
              </div>
              <div class="right-col">
                <p>Ngày nhập:</p>
                <p>${ngayNhap}</p>
                <div id="qr-${index}" class="qr-code mt-3"></div>
              </div>
            </div>
          </div>
            </div>

        `;
        $('.lablebarcode').append(labelHTML);

        new QRCode(document.getElementById(`qr-${index}`), {
            text: barcodechia,
            width: 100,
            height: 100
        });
    })

}
function sumKien() {
    var thucnhap = $thisTrDelete.find("td.thucnhap").text().trim();
    var $trKienGoc;
    var slGoc = 0;
    var totalKienChia = parseFloat(parseFloat(thucnhap).toFixed(2));

    let kiengoctrdelete = $thisTrDelete.data("kiengoc");

    $("#tblDataBody tr").each(function () {
        let $trRow = $(this);
        let kiengocchia = $trRow.find("td.sokien").text().trim();

        if (kiengocchia == kiengoctrdelete) {
            $trKienGoc = $trRow;
            slGoc = parseFloat($trRow.data("slnhap")) || 0;
            return false; // ⛔ Thoát vòng lặp ngay khi tìm thấy
        }
    });

    // Nếu tìm thấy dòng gốc thì cập nhật
    if ($trKienGoc) {
        let newValue = slGoc + totalKienChia;
        $trKienGoc.find("td.thucnhap").text(newValue);
        $trKienGoc.attr("data-slnhap", newValue);
        $trKienGoc.data("slnhap", newValue);
    }
}
var arrKienMax = []
function getMaxKien(trKienGoc) {
    arrKienMax = []
    $("#tblDataBody tr").each(function () {
        let kiengoc = $(this).data("kiengoc")
        let kienchia = $(this).find(".sokien").text().trim()
        if (trKienGoc == kiengoc) arrKienMax.push(kienchia)
    })

}
function showHidePopTablePD() {
    if ($(".popup-tablesearchPD").hasClass("active")) {
        $(".popup-tablesearchPD").removeClass("active")
    }
    else $(".popup-tablesearchPD").addClass("active")
}
$('#NPL_LO').on('change', function () {
    //const selected = $(this).val();
    const selectedOptions = Array.from(this.selectedOptions).map(option => option.value).join(';');
    trSolo = selectedOptions
    GetXuatHang(selectedOptions);

});

const inputSearch = document.querySelector('.item_inputsearchPD');
const tbody = document.getElementById('tblLo');
var currentRow = null;
inputSearch.addEventListener('input', function () {
    const keyword = inputSearch.value.trim().toLowerCase();


    Array.from(tbody.querySelectorAll('tr')).forEach(tr => {

        const tdSoLo = tr.querySelector('td[data-soloid]');
        if (!tdSoLo) {
            tr.style.display = '';
            return;
        }
        const soloText = tdSoLo.textContent.trim().toLowerCase();


        if (soloText.includes(keyword) || keyword === '') {
            tr.style.display = '';
        } else {
            tr.style.display = 'none';
        }
    });
});

async function GetMaHang() {
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetMaHang`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();

        let html = `<option value=""></option>`;
        data.map(item => {
            html += ` <option  data-tenhang="${item.TenHang}" data-makh="${item.MaKH}" data-tenkh="${item.TenKH}" value="${item.MaHang}">${item.Display}</option> `
        })
        $("#DH_PKL").html(html)


    } catch (error) {
        console.error(error.message);
    }
}

async function GetLo() {
    const url = `/api/PhieuXuatHangNPL/Get?Action=GetSoLo`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();

        const tbody = document.getElementById('tblLo');
        if (!tbody) {
            throw new Error('Không tìm thấy tbody với id="tblLo"');
        }
        tbody.innerHTML = '';
        data.forEach(item => {
            const tr = document.createElement('tr');


            const tdCheck = document.createElement('td');

            tdCheck.style.minWidth = "30px";
            tdCheck.style.maxWidth = "30px";
            tdCheck.style.whiteSpace = "nowrap";
            tdCheck.style.padding = "0";
            tdCheck.innerHTML = `<div class="d-flex justify-content-center align-items-center"> <input style="width:18px;height:18px" type="checkbox" ${item.isCheck ? 'checked' : ''}></div>`;
            tr.appendChild(tdCheck);

            // SoLo
            const tdSoLo = document.createElement('td');
            tdSoLo.setAttribute('data-soloid', item.SoLoID ?? '');
            tdSoLo.textContent = item.SoLo ?? '';
            tr.appendChild(tdSoLo);

            tbody.appendChild(tr);
        });
        tbody.addEventListener('change', function (event) {
            if (event.target && event.target.type === 'checkbox') {

                const checkboxes = tbody.querySelectorAll('input[type="checkbox"]:checked');

                const soloidArr = [];
                const solos = [];

                checkboxes.forEach(cb => {
                    const tr = cb.closest('tr');
                    const td = tr.querySelector('td[data-soloid]');
                    if (td) {
                        soloidArr.push(td.getAttribute('data-soloid'));
                        solos.push(td.textContent.trim());
                    }
                });


                const soloidString = soloidArr.join(';');
                document.getElementById('Lo_Select').value = solos.join('; ');
                trSolo = soloidString;
                GetCayVai()


            }
        });

    } catch (error) {
        console.error(error.message);
    }
}

async function GetLot(soLoID) {
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetLot&para1=` + soLoID;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();

        const selectEl = document.getElementById('NPL_LOT');


        selectEl.innerHTML = '<option value="" selected></option>';


        data.forEach(item => {
            const option = document.createElement('option');

            option.value = item.SoLoT;
            option.textContent = `${item.SoLoT}`;
            selectEl.appendChild(option);
        });


    } catch (error) {
        console.error(error.message);
    }
}


async function GetXuatHang() {
    $(".checkAllXH").prop("checked", false)
    let mahang = $("#CayVai_Selected option:selected").data("mahang");
    let makh = $("#CayVai_Selected option:selected").data("makh");
    var solo = $("#CayVai_Selected option:selected").data("solo");
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetXuatHang&para1=${solo}&para2=${mahang}&para3=${makh}&para4=${$("#CayVai_Selected").val()}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        arrXH = data
        const tbody = document.getElementById('tblDataBody');
        tbody.innerHTML = '';
        const checkLenghth = data.reduce((acc, cur) => acc + Number(cur.isCheck), 0);
        if (checkLenghth == data.length && data.length > 0)
            $("#thead").find("input.checkAllXH").prop("checked", true)

        else $("#thead").find("input.checkAllXH").prop("checked", false)
        data.forEach(item => {
            const tr = document.createElement('tr');
            setTrDataAttributes(tr, item, tdFields);
            tr.setAttribute("data-kiengoc", item.SoKien)
            tr.setAttribute("data-soluonggoc", item.ThucNhap)
            if (item.InBarCode == 2)
                tr.className = "highlight-row";
            const tdCheck = document.createElement('td');
            tdCheck.innerHTML = ` <div class="d-flex justify-content-center align-items-center"><input class="ipcheckbox" type="checkbox" ${item.isCheck ? 'checked' : ''}/></div>`;
            tr.appendChild(tdCheck);
            tr.innerHTML += `
                <td>${item.SoLo ?? ''}</td>
                <td class="phieuyc">${item.PYC ?? ''}</td>
                <td style="min-width: 250px;" class="chitiet">${item.ChiTiet ?? ''}</td>
                <td class="mau">${item.MauVT ?? ''}</td>
                <td class="sokien">${parseFloat(parseFloat(item.KienChia).toFixed(2)) ?? ''}</td>
               
                <td class="solot">${item.SoLoT ?? ''}</td>
                <td class="khosize">${item.KhoVai ?? ''}</td>
                 <td class="thucnhapT">${parseFloat(parseFloat(item.ThucNhap).toFixed(2)) || ''}</td>
                 <td class="thucnhap">${parseFloat(parseFloat(item.ThucNhapA).toFixed(2))}</td>
                <td class="donvi">${item.TenDVVT ?? ''}</td>
                <td class="ghichu">${item.GhiChu ?? ''}</td>
                    <td style="max-width: 60px;min-width: 50px;">
                            <div style="display: flex;justify-content: center; align-items: center;">
                           <input type = "checkbox" class="checkbc" style = "width: 18px; height: 18px;"   />
                          </div>
                    </td>
            `;
            const tdAction = document.createElement('td');
            const dataAttrs = createDataAttributes(item);
            tdAction.innerHTML = `<div style="display: flex;justify-content: center; align-items: center">
                <button style="width: 50px;display: flex;justify-content: center; align-items: center; background: #fff;height: 20px;" class="btn btn-success btn-sm btnChiaKien" ${dataAttrs}"><i style="font-size: 22px;" class="fa-solid fa-square-share-nodes"></i></button>

            </div>`;
            const tdActionA = document.createElement('td');
            if (item.ChuyenGoc == 0)
                tr.appendChild(tdAction);
            else tr.appendChild(tdActionA);

            tbody.appendChild(tr);
        });


        document.querySelectorAll('.btnChiaKien').forEach(btn => {
            btn.addEventListener('click', function () {
                const soLo = this.getAttribute('data-solo');
                const vatTu = this.getAttribute('data-chitiet');
                const mau = this.getAttribute('data-mauvt');
                const khoSize = this.getAttribute('data-khovai');
                const donVi = this.getAttribute('data-tendvvt');
                const soKien = this.getAttribute('data-sokien');
                const thucnhap = this.getAttribute('data-thucnhapA');
                document.getElementById('modalSoLo').textContent = soLo;
                document.getElementById('modalVatTu').textContent = vatTu;
                document.getElementById('modalMau').textContent = mau;
                document.getElementById('modalKhoSize').textContent = khoSize;
                document.getElementById('modalDonVi').textContent = donVi;
                document.getElementById('modalKienGoc').value = soKien;
                document.getElementById('modalSoKienTach').value = 1;
                document.getElementById('modalSoLuong').value = "";
                document.getElementById('modalThucNhap').textContent = thucnhap;
                const btnSave = document.getElementById('btnSaveTachKien');
                Object.keys(btnSave.dataset).forEach(key => delete btnSave.dataset[key]);
                Object.entries(this.dataset).forEach(([key, val]) => btnSave.dataset[key] = val);
                currentRow = btn.closest('tr');
                $('#myModal').modal('show');
            });
        });

        await getTfoot()
    } catch (error) {
        console.error(error.message);
    }
}
function createDataAttributes(item) {
    let result = '';
    for (let key in item) {
        if (item.hasOwnProperty(key)) {
            let value = item[key];
            if (typeof value !== 'object' && typeof value !== 'function') {
                result += ` data-${key.toLowerCase()}="${String(value).replace(/"/g, '&quot;')}"`;
            }
        }
    }
    return result;
}
function setTrDataAttributes(tr, data, tdFields) {
    Object.keys(data).forEach(key => {
        if (!tdFields.includes(key)) {
            tr.dataset[key.toLowerCase()] = data[key] ?? '';
        }
    });
}
async function tachKien() {
    try {
        var _sokientach = document.getElementById('modalSoKienTach').value;

        if (_sokientach == '' || _sokientach <= 0) {
            iziToast.warning({
                title: `Warning`,
                message: `Vui lòng nhập số kiện tách`,
                position: 'topRight'
            });
            return;
        }
        var _sl = document.getElementById('modalSoLuong').value;
        if (_sl == "") {
            iziToast.warning({
                title: `Warning`,
                message: `Vui lòng nhập số lượng`,
                position: 'topRight'
            });
            return;
        }
        var btn = document.getElementById('btnSaveTachKien');
        var data = btn.dataset;
        var maxKienTable = arrKienMax.sort(function (a, b) {
            var [a1, a2] = a.split('.').map(Number);
            var [b1, b2] = b.split('.').map(Number);
            if (a1 !== b1) return a1 - b1;
            return a2 - b2;
        });
        var maxKien = tangSoKien(maxKienTable[maxKienTable.length - 1])
        var _kienAPI = await getSoKien(data.manpl, data.soloid, data.kienchia)
        if (_kienAPI && _kienAPI > maxKien)
            maxKien = _kienAPI
        for (var i = 0; i < _sokientach; i++) {
            let barCode = `${barCodeGoc}${maxKien}`
            var solo = data.solo;
            var vattu = data.chitiet;
            var mau = data.mauvt;
            var khosize = data.khovai;
            var donvi = data.tendvvt;
            var sokien = 0;
            var sokien = maxKien
            var thucnhap = _sl;
            var solot = data.solot;
            var ghichu = $("#modalGhiChu").val();
            data.sokien = sokien;
            data.thucnhap = thucnhap;
            var tr = document.createElement("tr");
            tr.setAttribute("data-kiengoc", kiengocAttr)
            tr.setAttribute("data-soluonggoc", soluonggoc)
            tr.setAttribute("data-barcodegoc", barCodeGoc)
            tr.setAttribute("data-barcodechia", barCode)
            tr.setAttribute("data-inbarcode", 2)
            tr.setAttribute("data-slnhap", parseFloat(parseFloat(thucnhap).toFixed(2)))
            tr.setAttribute("data-ngaynk", ngayNK)
            tr.className = "highlight-row";
            tr.innerHTML = `
            <td><div class="d-flex justify-content-center align-items-center"><input style=""  class="ipcheckbox" type="checkbox" ></div></td>
            <td>${solo ?? ''}</td>
             <td class="phieuyc"></td>
            <td class="chitiet">${vattu ?? ''}</td>
            <td <td class="mau">${mau ?? ''}</td>
            <td class="sokien">${parseFloat(parseFloat(sokien).toFixed(2)) ?? ''}</td>

            <td class="solot">${solot ?? ''}</td>
            <td class="khosize">${khosize ?? ''}</td>
            <td class="thucnhapT">${parseFloat(parseFloat(soluonggoc).toFixed(2)) || ''}</td>
            <td class="thucnhap">${parseFloat(parseFloat(thucnhap).toFixed(2)) || ''}</td>
            <td class="donvi">${donvi ?? ''}</td>
            <td class="ghichu">${ghichu ?? ''}</td>
            <td style="max-width: 60px;min-width: 50px;">
                            <div style="display: flex;justify-content: center; align-items: center;">
                           <input type = "checkbox" class="checkbc" style = "width: 18px; height: 18px;"  checked />
                          </div>
                    </td>
           <td>
              <div class="d-flex justify-content-center align-items-center">
                <button
                  type="button"
                  class="btn btn-success btn-sm btnXoaKien d-flex justify-content-center align-items-center"
                  style="width:50px; height:20px;background: transparent; padding: 0; border: transparent; padding:0;"
                >
                  <i class="fa-solid fa-trash-xmark" style="font-size: 22px; color:#d800ff;margin-right: 6px;"></i>
                </button>
              </div>
            </td>
        
        `;
            document.getElementById('tblDataBody').appendChild(tr);
            maxKien = String(parseFloat(maxKien) + 0.1);
        }
        let tdThucNhap = currentRow.querySelector('td:nth-child(10)');
        let thucNhap = Number(tdThucNhap.textContent);
        let soLuongTach = (_sokientach * _sl);
        if (soLuongTach > thucNhap || soLuongTach <= 0) {
            console.log('Số lượng tách bị lỗi ' + soLuongTach);
            $('#myModal').modal('hide');
            return;
        }
        let soMoi = thucNhap - soLuongTach;
        tdThucNhap.textContent = parseFloat(soMoi.toFixed(2));

        let btnTach = currentRow.querySelector('.btnChiaKien');
        btnTach.setAttribute('data-thucnhap', parseFloat(soMoi.toFixed(2)));
        $('#myModal').modal('hide');
        let trRow = $(tdThucNhap.closest("tr")); // <-- bọc bằng $ để dùng jQuery
        trRow.attr("data-slnhap", parseFloat(soMoi.toFixed(2)));
        trRow.data("slnhap", parseFloat(soMoi.toFixed(2)));
        sortTableByFloatClassDesc("sokien");
        getTfoot()
    } catch (e) {
        console.log(e);
    }


}

function sortTableByFloatClassDesc(className) {
    var $tbody = $("#tblDataBody");
    var $rows = $tbody.find("tr").get();

    $rows.sort(function (a, b) {
        var valA = parseFloat($(a).find('.' + className).text());
        var valB = parseFloat($(b).find('.' + className).text());
        valA = isNaN(valA) ? 0 : valA;
        valB = isNaN(valB) ? 0 : valB;
        return valA - valB;
    });

    $.each($rows, function (idx, row) {
        $tbody.append(row);
    });
}

function tangSoKien(soKien) {
    soKien = soKien.toString().trim();
    var parts = soKien.split(".");
    if (parts.length === 1) {
        var soTruocCham = parseInt(parts[0]);
        if (!isNaN(soTruocCham)) {
            return soTruocCham + ".1";
        }
    }
    else if (parts.length === 2) {
        var soTruocCham = parseInt(parts[0]);
        var soSauCham = parseInt(parts[1]);
        if (!isNaN(soTruocCham) && !isNaN(soSauCham)) {
            return soTruocCham + "." + (soSauCham + 1);
        }
    }

}
async function getSoKien(manpl, soloid, kiengoc) {
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetMaxKien&para1=${manpl}&para2=${soloid}&para3=${kiengoc}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();

        const soKien = data.length == 0 ? 0 : data[0].SoKien;

        return soKien;



    } catch (error) {
        console.error(error.message);
        return 0;
    }
}
function Save() {
    var ArrSave = []
    var dataFirst = arrXH[0]
    //if ($("#DH_PKL").val() == "") {
    //    iziToast.warning({
    //        title: `Warning`,
    //        message: `Vui lòng chọn mã hàng`,
    //        position: 'topRight'
    //    });
    //    return
    //}
    let tenhang = $("#DH_PKL option:selected").data("tenhang");
    let mahang = $("#CayVai_Selected option:selected").data("mahang");
    let makh = $("#CayVai_Selected option:selected").data("makh");
    let tenkh = $("#DH_PKL option:selected").data("tenkh");

    $("#tblDataBody tr").each(function () {
        let $trRow = $(this)
        var barcodechia = $trRow.data("barcodechia")
        let barCode = $trRow.data("barcodegoc")
        const soLoObject = {
            SoLoID: dataFirst.SoLoID,
            SoLo: dataFirst.SoLo,
            MaKH: makh,
            PhieuYC: $trRow.find("td.phieuyc").text().trim(),
            MaHang: mahang,
            BarCodeGoc: barCode,
            MaNPL: dataFirst.MaNPL,
            CayVai: dataFirst.ChiTiet,
            SoLot: dataFirst.SoLoT,
            KhoVai: dataFirst.KhoVai,
            KhoVaiID: dataFirst.KhoVaiID,
            DonVi: dataFirst.TenDVVT,
            MaDonVi: dataFirst.MaDVVT,
            SoKien: $trRow.find("td.sokien").text().trim(),
            KienGoc: $trRow.data("kiengoc"),
            SLGoc: $trRow.data("soluonggoc"),
            SLNhap: $trRow.data("slnhap"),
            isCheck: $trRow.find(".ipcheckbox").is(":checked") ? 1 : 0,
            GhiChu: $trRow.find("td.ghichu").text().trim(),
            BarCode: barcodechia


        };
        ArrSave.push(soLoObject)
    })
    ApiSave(ArrSave)

}
async function ApiSave(arrSave) {
    var dataUser = !window.CefSharp ? userNameSave : dataUser = userName

    const request = new Request(`/api/PhieuXuatHangNPL/Post?action=Post&para1=${dataUser}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSave),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data == "True") {
        iziToast.success({
            message: `Lưu thành công`,
            position: 'topRight',
            timeout: 2000
        });
        returnThucNhap()
    }
    else {
        iziToast.warning({
            message: `Lưu thất bại`,
            position: 'topRight',
            timeout: 2500
        });
    }
}
function returnThucNhap() {
    $("#tblDataBody tr").each(function () {
        let $trRow = $(this)
        if ($trRow.find(".ipcheckbox").is(":checked")) {
            $trRow.find("td.thucnhap").text(0)
        }
        $trRow.find(".btnXoaKien").addClass("hide-important");
    })

}
async function GetCayVai() {
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetCayVai&para1=${trSolo}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        let html = ``;
        data.map(item => {
            var selected = manplQuet == item.MaNPL ? "selected" : ""
            html += ` <option data-makh="${item.MaKhachHang}" data-mahang='${item.MaHang}' data-slcl="${item.TotalCat}" data-mavt="${item.MaVT}" ${selected} data-solo="${item.SoLoID}" value="${item.MaNPL}">${item.Display}</option> `
        })
        $("#CayVai_Selected").html(html)

        await GetPhieuYeuCau();
        await GetXuatHang();



    } catch (error) {
        console.error(error.message);
        return 0;
    }
}
function getTfoot() {

    var html = textSpanTd()
    let htmlTfoot = `
                     <div class="tdtfooter">${html}</div>

        `
    $("#tfootML").html(htmlTfoot)
}
function textSpanTd() {
    let html = ""
    let htmlDC = ""
    let htmmlSpan = ""
    dataPYC.map((x, index) => {
        let total = 0;
        $('#tblDataBody tr').each(function () {
            const $phieuCell = $(this).find('td.phieuyc').text();
            if (x.MaPhieu == $phieuCell) {
                const soLuong = parseFloat($(this).data("slnhap")) || 0;
                total += soLuong;
            }
        });

        var metcl = parseFloat(x.SoMet) - total
        var metclt = parseFloat(metcl) <= parseFloat(x.SoMetCL) ? metcl : x.SoMetCL - total



        var span = `<span class="${x.MaPhieu}">${x.MaPhieu}: <span class='yeucaucong'>${parseFloat(metclt.toFixed(2))}</span> m ${index + 1 == dataPYC.length ? "" : " -- "}</span>`
        var spanDC = `<span class="${x.MaPhieu}">${x.MaPhieu}: <span class='yeucautru'>${parseFloat(total.toFixed(2))}</span> m ${index + 1 == dataPYC.length ? "" : " -- "}</span>`
        html += span
        htmlDC += spanDC
    })
    htmmlSpan = `<div class="col-12 m-0 p-0"><span> Số mét đã cấp :</span> ${htmlDC}</div>
            <div class="col-12 m-0 p-0"><span> Số mét còn lại :</span> ${html}</div>
        
    `

    return htmmlSpan
}
//<div class="col-6">Số met đã cắt <span> ${x.MaPhieu}: <span>${total}</span> m ${index + 1 == dataPYC.length ? "" : " -- "}</span></div>
var dataPYC = []
async function GetPhieuYeuCau() {
    var solo = $("#CayVai_Selected option:selected").data("solo");
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetPhieuYeuCauNPL&para1=${$("#CayVai_Selected").val()}&para2=${solo}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        let html = ``;
        data.map(item => {
            var selected = maphieuyc == item.MaPhieu ? "selected" : ""
            html += ` <option ${selected} data-slcl="${item.SoMetCL}" value="${item.MaPhieu}">${item.Display}</option> `
        })
        $("#PhieuYeuCau_Selected").html(html)
        dataPYC = []
        dataPYC = data

        if (data.length > 0)
            SoMet = data[0].SoMetCL


    } catch (error) {
        console.error(error.message);
        return 0;
    }
}
function sumMetCheck() {
    var totalMetCheck = 0;
    $("#tblDataBody tr").each(function () {
        let $trRow = $(this)
        if ($trRow.find("input.ipcheckbox").is(":checked")) {
            var thucnhap = parseFloat($trRow.data("slnhap"))
            totalMetCheck += thucnhap
        }
    })
    $("#thucnhaptext").text(totalMetCheck)
}
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
                return;
            }

            lastScanTime = now;

            var loSelect = $('#Lo_Select').val();
            if (loSelect == "") {
                getCheck(decodedText);
                $("#QRScan").val("")
            } else {
                //if ($("#DH_PKL").val() == "") {
                //    iziToast.warning({
                //        title: `Warning`,
                //        message: `Vui lòng chọn mã hàng`,
                //        position: 'topRight'
                //    });
                //    PlayAudioError()
                //    return
                //}
                GetCheckBarCode(decodedText)
                $("#QRScan").val("")
            }





        },
        (errorMessage) => {
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
function GetCheckBarCode(mabarCode) {
    var checkbarcode = false
    $("#tblDataBody tr").each(function () {
        let $trRow = $(this)
        let barcode = $trRow.data("barcodechia")

        if (mabarCode == barcode) {
            $("#cayvaitext").text($trRow.find("td.chitiet").text())
            $("#ngaynktext").text($trRow.data("ngaynk"))
            $("#kientext").text($trRow.find("td.sokien").text())
            $("#thucnhaptext").text($trRow.find("td.thucnhap").text())
            if ($trRow.find("input.ipcheckbox").is(":checked")) {
                $("#QRScan").val("")
                iziToast.warning({
                    title: 'Warning',
                    message: 'Kiện đã được check xuất hàng .',
                    position: 'topRight'
                });
                checkbarcode = true
                PlayAudioError()
                return false
            }
            else {
                var $row = $trRow


                var giaTriThucNhap = parseFloat($row.data("slnhap")).toFixed(2);
                let sometCL = $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucaucong").text().trim()
                let sometC = $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucautru").text().trim()

                if (sometCL - giaTriThucNhap < 0) {
                    iziToast.warning({
                        title: 'Warning',
                        message: 'Quá số lượng thực nhập yêu cầu. Không thể xuất hàng!!!.',
                        position: 'topRight'
                    });
                    PlayAudioError()
                    return false
                }

                var sumTfoot = parseFloat(sometCL) - parseFloat(giaTriThucNhap)
                var sumTfootT = parseFloat(sometC) + parseFloat(giaTriThucNhap)
                $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucaucong").text(parseFloat(sumTfoot.toFixed(2)))
                $(".tdtfooter").find(`span.${$("#PhieuYeuCau_Selected").val()}`).find("span.yeucautru").text(parseFloat(sumTfootT.toFixed(2)))

                $row.find("td.phieuyc").text($("#PhieuYeuCau_Selected").val())
                PlayAudio()

                $("#QRScan").val("")
                $trRow.find(".ipcheckbox").prop("checked", true)
                checkbarcode = true

                return false
            }
        }
    })
    if (!checkbarcode) {
        iziToast.warning({
            title: 'Warning',
            message: 'Barcode không khớp trong danh sách khai báo .',
            position: 'topRight'
        });
        PlayAudioError()
        $("#QRScan").val("")
    }
}
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

$(document).on('keydown', async function (e) {

    if (e.key === 'Enter') {
        if ($('#QRScan:focus').length > 0) {
            var barcodeNow = e.target.value.trim();
            var loSelect = $('#Lo_Select').val();
            if (loSelect == "") {
                getCheck(barcodeNow);
                $("#QRScan").val("")
            } else {
                //if ($("#DH_PKL").val() == "") {
                //    PlayAudioError()
                //    iziToast.warning({
                //        title: `Warning`,
                //        message: `Vui lòng chọn mã hàng`,
                //        position: 'topRight'
                //    });

                //    return
                //}
                GetCheckBarCode(e.target.value.trim())
                $("#QRScan").val("")
            }


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

function inbarcode() {
    GetDataBarCode()
    $("#myModalIn").modal("show")
}

async function getCheck(barcodeNow) {
    manplQuet = ""
    soloidQuet = ""
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetSoLoBarCode&para1=${barcodeNow}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            console.log('response error');
            return;
        }
        const data = await response.json();
        if (data.length == 0) {
            iziToast.warning({
                title: 'Warning',
                message: 'Barcode không khớp trong danh sách khai báo .',
                position: 'topRight'
            });
            PlayAudioError()
            $("#QRScan").val("")
            return
        }
        if (data[0].PYC == 0) {
            iziToast.warning({
                title: 'Warning',
                message: 'Cây vải này không được yêu cầu cắt!!!.',
                position: 'topRight'
            });
            PlayAudioError()
            $("#QRScan").val("")
            return
        }
        PlayAudio()
        manplQuet = data[0].MaNPL;
        soloidQuet = data[0].SoLoID;
        maphieuyc = data[0].PhieuYC
        await checkSoLo();

    } catch (error) {
        console.error(error.message);
        return 0;
    }
}
function checkSoLo() {
    var tbody = document.getElementById('tblLo');
    if (!tbody) return;

    var tr = tbody.querySelector('td[data-soloid="' + soloidQuet + '"]');
    if (tr) {
        var checkbox = tr.parentNode.querySelector('input[type="checkbox"]');
        if (checkbox) {
            checkbox.checked = true;
            checkbox.dispatchEvent(new Event('change', { bubbles: true }));
        }
    }
}

// tab2 -------------------
async function GetPhieuNhapKho() {
    var ngaythang = moment($(".datepicker").val(), "DD-MM-YYYY").format("YYYY-MM-DD")
    const url = `/api/PhieuXuatHangNPL/Get?action=GetPhieuYeuCau&para1=${ngaythang}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        if (data.length == 0) {
            $("#phieuxuatkho").empty()

        }
        let html = ``;
        data.map(x => {
            html += `
                <option value="${x.MaPhieu}">${x.MaPhieu}</option>
            `
        })
        $("#phieuxuatkho").html(html)
        GetDataPhieuXuatKho()
    } catch (error) {
        console.error(error.message);
    }
}
async function GetDataPhieuXuatKho() {
    const url = `/api/PhieuXuatHangNPL/Get?action=GetDataPhieuNhapKho&para1=${$("#phieuxuatkho").val()}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        GetData(data)
    } catch (error) {
        console.error(error.message);
    }
}
function GetData(data) {
    arrBaoCao = data;
    const columns = [
        {
            dataField: "ChiTiet",
            caption: "Chi tiết",
            groupIndex: 0,

            width: 50,
            cssClass: 'col-khu d-none t',
        },
        {

            dataField: "MaLenh",
            caption: "Mã lệnh",
            allowSorting: false,
            cssClass: 'col-xn text-center malenhtd',
            headerCellTemplate: function (container) {
                $("<div>")
                    .html("Mã <br/> lệnh")
                    .appendTo(container);
            }
        },
        {

            dataField: "KhachHang",
            caption: "Khách hàng",
            allowSorting: false,
            cssClass: 'col-xn text-center khachhangtd',
        },
        {
            dataField: "MaHang",
            caption: "Mã hàng",
            cssClass: 'col-khu text-center mahangtd mw-100',
        },
        {
            dataField: "Code_TNC",
            caption: "Code TNC",
            cssClass: 'col-dep text-center codetnctd mw-100',
        },
        {
            dataField: "ColorTong",
            caption: "Màu đơn hàng",
            cssClass: 'col-dep text-center mw-200',
        },
        {
            dataField: "TenNhom",
            caption: "Loại vải",
            cssClass: 'col-dep text-center mw-100',
        },
        {
            dataField: "MauVT",
            caption: "Màu",
            cssClass: 'col-th text-center mavttd mw-100',
        },
        {
            dataField: "BanCat",
            caption: "STT",
            cssClass: 'col-mahang text-center mw-80',
        },
        {
            dataField: "SoDo",
            caption: "Sơ đồ",
            cssClass: 'col-kh text-center mw-80',
        },
        {
            dataField: "Kho",
            caption: "Khổ",
            cssClass: 'col-th text-center mw-80',
        },
        {
            dataField: "Dai",
            caption: "Dài SĐ",
            cssClass: 'col-th text-center mw-80',
            headerCellTemplate: function (container) {
                $("<div>")
                    .html("Dài <br/> SĐ")
                    .appendTo(container);
            }
        },
        {
            dataField: "SoLuong",
            caption: "Số lượng",
            cssClass: 'col-th text-center mw-80',
            headerCellTemplate: function (container) {
                $("<div>")
                    .html("Số <br/> lượng")
                    .appendTo(container);
            }
        },

        {
            dataField: "SoLop",
            caption: "Số lớp ",
            cssClass: 'col-th text-center mw-80',
            headerCellTemplate: function (container) {
                $("<div>")
                    .html("Số <br/> lớp")
                    .appendTo(container);
            }
        }
        ,
        {
            dataField: "SLBTP",
            caption: "SL BTP",
            dataType: "number",
            cssClass: 'col-th text-center mw-80',
            headerCellTemplate: function (container) {
                $("<div>")
                    .html("SL <br/> BTP")
                    .appendTo(container);
            }
        },
        {

            dataField: "SoMet",
            caption: "Số mét yêu cầu",
            dataType: "number",
            cssClass: 'col-th text-center mw-80',
            headerCellTemplate: function (container) {
                $("<div>")
                    .html(" Tổng <br/> mét")
                    .appendTo(container);
            }

        },
        {

            dataField: "TotalCat",
            caption: "Số mét đã cắt",
            dataType: "number",
            cssClass: 'col-th text-center dacattd',
            headerCellTemplate: function (container) {
                $("<div>")
                    .html(" Mét <br/> TH")
                    .appendTo(container);
            }

        },
    ];
    function generateGroupSummary(cols) {
        const groupItems = [];
        function scan(columns) {
            columns.forEach(col => {
                if (col.columns) {
                    scan(col.columns);
                }
                else if (col.dataType === "number" && col.dataField) {
                    const field = col.dataField;

                    if (field == "SoMet") {
                        groupItems.push({
                            name: "SoMet_T_SUM",
                            summaryType: "custom",
                            showInColumn: "SoMet",
                            showInGroupFooter: true,
                            valueFormat: { type: "fixedPoint", precision: 0 },
                            customizeText: e =>
                                e.value != null
                                    ? e.value.toLocaleString("en-EN")
                                    : "0"
                        });
                    }
                    else if (field == "SLBTP") {
                        groupItems.push({
                            name: "SLBTP_T_SUM",
                            summaryType: "custom",
                            showInColumn: "SLBTP",
                            showInGroupFooter: true,
                            valueFormat: { type: "fixedPoint", precision: 0 },
                            customizeText: e =>
                                e.value != null
                                    ? e.value.toLocaleString("en-EN")
                                    : "0"
                        });
                    }
                    else if (field == "TotalCat") {
                        groupItems.push({
                            name: "TotalCat_T_SUM",
                            summaryType: "custom",
                            showInColumn: "TotalCat",
                            showInGroupFooter: true,
                            valueFormat: { type: "fixedPoint", precision: 0 },
                            customizeText: e =>
                                e.value != null
                                    ? e.value.toLocaleString("en-EN")
                                    : "0"
                        });
                    }
                }
            });
        }

        scan(cols);
        return groupItems;
    }
    function customSummaryHandler(options) {
        if (options.name === "SoMet_T_SUM") {
            if (options.summaryProcess === "start") {
                options.totalValue = 0;
            }
            if (options.summaryProcess === "calculate") {
                options.totalValue += options.value.SoMet || 0;
            }
        }
        if (options.name === "SLBTP_T_SUM") {
            if (options.summaryProcess === "start") {
                options.totalValue = 0;
            }
            if (options.summaryProcess === "calculate") {
                options.totalValue += options.value.SLBTP || 0;
            }
        }
        if (options.name === "TotalCat_T_SUM") {
            if (options.summaryProcess === "start") {
                options.totalValue = 0;
            }
            if (options.summaryProcess === "calculate") {
                options.totalValue += options.value.TotalCat || 0;
            }
        }
    }

    $("#tblBaoCao").dxDataGrid({
        width: '100%',
        dataSource: arrBaoCao,
        columns: columns,
        noDataText: "Chưa có dữ liệu",
        showBorders: true,
        rowAlternationEnabled: true,
        columnAutoWidth: true,
        columnHidingEnabled: false,
        allowColumnResizvirtualing: true,
        allowColumnReordering: true,
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column && e.cellElement) {
                const $cell = $(e.cellElement);
                const cellText = $cell.text().trim();
                const hasOverflow = $cell[0].scrollWidth > $cell.innerWidth();
                if (hasOverflow) {
                    $cell.attr("title", cellText);
                }
            }
        },
        scrolling: {
            mode: "standard" // Hoặc "virtual" nếu bạn cần xử lý dữ liệu lớn
        },
        columnFixing: {
            enabled: true
        },
        scrolling: {
            mode: "standard", // hoặc "virtual"
            useNative: true,
            showScrollbar: "always"
        },
        headerFilter: {
            visible: true

        },
        paging: { enabled: false },
        searchPanel: {
            visible: true,
            highlightCaseSensitive: true,
            width: "175px",
        },

        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },


        groupPanel: { visible: true },
        groupFooter: { visible: true },
        showBorders: true,
        summary: {
            groupItems: generateGroupSummary(columns),
            calculateCustomSummary: customSummaryHandler,
        },

        onContentReady: function (e) {
            $(".dx-datagrid-headers td").css({
                "background-color": "#5da566",
                "color": "#fff",
                "text-align": "center",
                "vertical-align": "middle",
                "font-weight": "700"
            });

            $(".dx-group-footer td, .dx-footer td").css({
                "background-color": "#f0f8ff",
                "font-weight": "bold",
                "color": "#003366"
            });

            $(".dx-group-row").css({
                "text-align": "left",
                "font-weight": "bold",
                "background-color": "#e6f2ff"
            });
            mergeColumns(["malenhtd", "khachhangtd", "mahangtd", "codetnctd"]);
            merge2Class("dacattd", "mavttd")
        },

    });

    $("#Layer_1").click()
}
function mergeColumns(classNames) {
    let $rows = $('table tbody tr.dx-column-lines');
    classNames.forEach(className => {
        let previousCell = null;
        let rowspan = 1;

        $rows.each(function () {
            let $currentCell = $(this).find('td.' + className);
            let currentText = $currentCell.text().trim();

            if (previousCell === null) {
                previousCell = $currentCell;
                rowspan = 1;
            } else if (currentText === previousCell.text().trim()) {
                rowspan++;
                $currentCell.addClass('d-none');
                previousCell.attr('rowspan', rowspan);
                previousCell.addClass("textMidle");
            } else {
                previousCell = $currentCell;
                rowspan = 1;
            }
        });
    });
}
function merge2Class(className1, className2) {
    let $rows = $('table tbody tr.dx-column-lines');
    let previousText = null;
    let previousCell = null;
    let rowspan = 1;
    let colSumDaCat = 0;

    $rows.each(function () {
        let $currentCell1 = $(this).find('td.' + className1);
        let $currentCell2 = $(this).find('td.' + className2);

        let currentText = $currentCell1.text().trim() + '|' + $currentCell2.text().trim();
        let cellValue = parseFloat($currentCell1.text().trim()) || 0;

        // Chỉ cộng dồn giá trị của ô đầu tiên trong nhóm gộp
        if (previousText === null || currentText !== previousText) {
            if ($(this).hasClass('dx-datagrid-group-footer')) return;
            colSumDaCat += cellValue;
        }

        if (previousText === null) {
            previousText = currentText;
            previousCell = $currentCell1;
            rowspan = 1;
        } else if (currentText === previousText) {
            rowspan++;
            $currentCell1.addClass('d-none');
            previousCell.attr('rowspan', rowspan);
            previousCell.addClass("textMidle");
        } else {
            previousText = currentText;
            previousCell = $currentCell1;
            rowspan = 1;
        }
    });

    // Cập nhật tổng số mét đã cắt
    $(".dx-datagrid-summary-item[aria-label*='Số mét đã cắt']").text(colSumDaCat);
}

//end