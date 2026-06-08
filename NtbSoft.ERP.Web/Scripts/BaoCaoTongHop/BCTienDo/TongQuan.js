$(function () {
    if (sessionStorage.getItem("BranchId") == null) {
       
    } else {
        showLoading();
        try {

            let bcTienDo = function () {
                let todate = formatStrYMD($("#todate").val());
                let fromdate = formatStrYMD($("#todate").val());
                $(".tbody").empty();
                $(".tfoot").empty();
                let BranchID = sessionStorage.getItem("BranchId");
                let response = RunStored_GetData(BranchID, "SP_ERP_BAOCAOTIENDOSXREPORT", `@Action = 'GET' , @FromDate = '${fromdate}' ,@ToDate = '${todate}' ,@UserName = '${localStorage.getItem('username1')}' `);
                if (response != null) {
                    hideLoading();
                    renderTable(response);
                    Mergecell();

                } else {
                    Swal.fire({
                        title: 'Thông báo',
                        text: 'Không có dữ liệu ở thời gian được chọn!',
                        icon: 'info',
                        confirmButtonText: 'OK'
                    });
                    return;
                }
            };

            let renderTable = function (data) {
                let html = '';
                let html2 = "";
                let filterData = data.filter((item) => {
                    return item.Name != "Down"
                })
                let comat = 0, tong = 0, slkh = 0, sltinhluong = 0, sltrongthang = 0, thcat = 0,
                    lkcat = 0, thbtp = 0, lkbtp = 0, tonbtp = 0, dongia = 0, rcth = 0, rclk = 0,
                    radtth = 0, radtlk = 0, khcl = 0, nhtth = 0, nhtlk = 0, tontpchuyen = 0, uith = 0,
                    uilk = 0, tonui = 0, kiemth = 0, kienlk = 0, kiemlkdat = 0, ukiem = 0, hanghu = 0,
                    dtth = 0, dtlk = 0, dtton = 0, htdtth = 0, htdtlk = 0, chenhlech = 0;

                let variables = {
                    slkh: 0, sltinhluong: 0, sltrongthang: 0, thcat: 0, lkcat: 0,
                    thbtp: 0, lkbtp: 0, tonbtp: 0, dongia: 0, rcth: 0, rclk: 0, radtth: 0, radtlk: 0, khcl: 0, nhtth: 0,
                    nhtlk: 0, tontpchuyen: 0, uith: 0, uilk: 0, tonui: 0, kiemth: 0, kienlk: 0, kiemlkdat: 0, ukiem: 0,
                    hanghu: 0, dtth: 0, dtlk: 0, dtton: 0, htdtth: 0, htdtlk: 0, chenhlech: 0
                };

                for (let i = 0; i < filterData.length; i++) {
                    let d = filterData[i];

                    variables.slkh += d.SLKH; variables.sltinhluong += d.SLDLL; variables.sltrongthang += d.SLTT; variables.thcat += d.SLCut_TH; variables.lkcat + d.SLCut_LK;
                    variables.thbtp += d.BTP_TH; variables.lkbtp += d.BTP_LK; variables.tonbtp += d.SLCut_LK - d.BTP_LK; variables.dongia += d.DonGia;
                    variables.rcth += d.RC_HDTH; variables.rclk += d.RC_HDLK; variables.radtth += d.DT_RCTH; variables.radtlk += d.DT_RCLK;
                    variables.khcl += d.KH_ConLai; variables.nhtth += d.NTP_TH; variables.nhtlk += d.NTP_LK; variables.tontpchuyen += d.Ton_TPChuyen;
                    variables.uith += d.UI_TH; variables.uilk += d.UI_LK; variables.tonui += d.Ton_UI; variables.kiemth += d.SU_TH;
                    variables.kienlk += d.SU_LK; variables.kiemlkdat += d.LKHangDat; variables.ukiem += d.TonKiem; variables.hanghu += d.HangHu;
                    variables.dtth += d.GX_TH; variables.dtlk += d.GX_LK; variables.dtton += d.Ton_TPHT; variables.htdtth += d.DT_THHT;
                    variables.htdtlk += d.DT_LKHT; variables.chenhlech += d.ChenhLech;

                    comat += d.NoWorked; tong += d.HC; slkh += d.SLKH; sltinhluong += d.SLDLL; sltrongthang += d.SLTT; thcat += d.SLCut_TH; lkcat += d.SLCut_LK;
                    thbtp += d.BTP_TH; lkbtp += d.BTP_LK; tonbtp += d.SLCut_LK - d.BTP_LK; dongia += d.DonGia; rcth += d.RC_HDTH; rclk += d.RC_HDLK; radtth += d.DT_RCTH; radtlk += d.DT_RCLK;
                    khcl += d.KH_ConLai; nhtth += d.NTP_TH; nhtlk += d.NTP_LK; tontpchuyen += d.Ton_TPChuyen; uith += d.UI_TH; uilk += d.UI_LK; tonui += d.Ton_UI; kiemth += d.SU_TH;
                    kienlk += d.SU_LK; kiemlkdat += d.LKHangDat; ukiem += d.TonKiem; hanghu += d.HangHu; dtth += d.GX_TH; dtlk += d.GX_LK; dtton += d.Ton_TPHT; htdtth += d.DT_THHT;
                    htdtlk += d.DT_LKHT; chenhlech += d.ChenhLech;
                    let mamau = d.MaMau.replace("/", "/ ")

                    let table = `
                    <td class="classContentDungYen leftCol1"> ${d.Name} </td >
                    <td class="classContentDungYen leftCol2"> ${d.NoWorked} </td >
                    <td class="classContentDungYen leftCol3"> ${d.HC} </td>
                    <td class="classContentDungYen leftCol4"> ${d.MaHang} </td>
                    <td class="classContentDungYen leftCol5"> ${mamau} </td>
                    <td class="classContentDungYen leftCol6"> ${d.SLKH} </td>
                    <td class="classContentDungYen leftCol7"> ${d.SLDLL} </td>
                    <td class="classContentDungYen leftCol8"> ${d.SLTT} </td>
                    <td>${d.NgayCat == null ? 0 : d.NgayCat} </td>
                    <td>${d.SLCut_TH} </td>
                    <td>${d.SLCut_LK} </td>
                    <td>${d.NgayBTP} </td>
                    <td>${d.BTP_TH} </td>
                    <td>${d.BTP_LK} </td>
                    <td>${d.SLCut_LK - d.BTP_LK} </td>
                    <td>${d.DonGia} </td>
                    <td>${d.RC_HDTH} </td>
                    <td>${d.RC_HDLK} </td>
                    <td>${d.DT_RCTH} </td>
                    <td>${d.DT_RCLK} </td>
                    <td>${d.KH_ConLai} </td>
                    <td>${d.NTP_TH} </td>
                    <td>${d.NTP_LK} </td>
                    <td>${d.Ton_TPChuyen} </td>
                    <td>${d.UI_TH} </td>
                    <td>${d.UI_LK} </td>
                    <td>${d.Ton_UI} </td>
                    <td>${d.SU_TH} </td>
                    <td>${d.SU_LK} </td>
                    <td>${d.LKHangDat} </td>
                    <td>${d.TonKiem} </td>
                    <td>${d.HangHu} </td>
                    <td>${d.GX_TH} </td>
                    <td>${d.GX_LK} </td>
                    <td>${d.Ton_TPHT} </td>
                    <td>${d.DT_THHT} </td>
                    <td>${d.DT_LKHT} </td>
                    <td>${d.ChenhLech} </td>
            `

                    if (d.Name === (filterData[i + 1]?.Name) ?? "aa") {
                        html += `<tr class=""> ${table} </tr> `
                    }
                    else {
                        html += `
                    <tr class=""> ${table}</tr>
                        <tr >
                    <td class="classTotalLine total" colspan="5" style = "left:0px;"> tổng cộng</td>
                    <td class="leftCol6 total sticky"> ${variables.slkh}</td>
                    <td class="leftCol7 total sticky"> ${variables.sltinhluong} </td>
                    <td class="leftCol8 total sticky"> ${variables.sltrongthang} </td>
                    <td class="total"></td>
                    <td class="total">${variables.thcat} </td>
                    <td class="total">${variables.lkcat} </td>
                    <td class="total"></td>
                    <td class="total">${variables.thbtp} </td>
                    <td class="total">${variables.lkbtp} </td>
                    <td class="total">${variables.tonbtp} </td>
                    <td class="total">${parseInt(variables.dongia)}</td>
                    <td class="total">${variables.rcth} </td>
                    <td class="total">${variables.rclk} </td>
                    <td class="total">${parseInt(variables.radtth)} </td>
                    <td class="total">${parseInt(variables.radtlk)} </td>
                    <td class="total">${variables.khcl} </td>
                    <td class="total">${variables.nhtth} </td>
                    <td class="total">${variables.nhtlk} </td>
                    <td class="total">${variables.tontpchuyen} </td>
                    <td class="total">${variables.uith} </td>
                    <td class="total">${variables.uilk} </td>
                    <td class="total">${variables.tonui} </td>
                    <td class="total">${variables.kiemth} </td>
                    <td class="total">${variables.kienlk} </td>
                    <td class="total">${variables.kiemlkdat} </td>
                    <td class="total">${variables.ukiem} </td>
                    <td class="total">${variables.hanghu} </td>
                    <td class="total">${variables.dtth} </td>
                    <td class="total">${variables.dtlk} </td>
                    <td class="total">${variables.dtton} </td>
                    <td class="total">${parseInt(variables.htdtth)} </td>
                    <td class="total">${parseInt(variables.htdtlk)} </td>
                    <td class="total">${parseInt(variables.chenhlech)} </td>
                      </tr > `
                        for (let prop in variables) {
                            variables[prop] = 0;
                        }
                    }
                }
                html2 +=
                    `
            <tr class="">
                    <td class="classContentDungYen leftCol1">Tổng</td>
                    <td class="classContentDungYen leftCol2"> ${comat} </td>
                    <td class="classContentDungYen leftCol3"> ${tong} </td>
                    <td class="classContentDungYen leftCol4"></td>
                    <td class="classContentDungYen leftCol5"></td>
                    <td class="classContentDungYen leftCol6"> ${slkh} </td>
                    <td class="classContentDungYen leftCol7"> ${sltinhluong} </td>
                    <td class="classContentDungYen leftCol8"> ${sltrongthang} </td>
                    <td></td>
                    <td>${thcat} </td>
                    <td>${lkcat} </td>
                    <td></td>
                    <td>${thbtp} </td>
                    <td>${lkbtp} </td>
                    <td>${tonbtp} </td>
                    <td>${parseInt(dongia)}</td>
                    <td>${rcth} </td>
                    <td>${rclk} </td>
                    <td>${radtth.toFixed(2)} </td>
                    <td>${radtlk.toFixed(2)} </td>
                    <td>${khcl} </td>
                    <td>${nhtth} </td>
                    <td>${nhtlk} </td>
                    <td>${tontpchuyen} </td>
                    <td>${uith} </td>
                    <td>${uilk} </td>
                    <td>${tonui} </td>
                    <td>${kiemth} </td>
                    <td>${kienlk} </td>
                    <td>${kiemlkdat} </td>
                    <td>${ukiem} </td>
                    <td>${hanghu} </td>
                    <td>${dtth} </td>
                    <td>${dtlk} </td>
                    <td>${dtton} </td>
                    <td>${htdtth} </td>
                    <td>${parseInt(htdtlk)} </td>
                    <td>${parseInt(chenhlech)} </td>
                  </tr>
            `
                $(".tfoot").html(html2)
                $(".tbody").html(html);
            };

            bcTienDo();

            $("#todate").on("change", function () {
                showLoading();
                try {
                    bcTienDo();
                    hideLoading();
                } catch (error) {
                    hideLoading();
                    showPopupError();
                }
            });

            $(".ex").on("click", function () {
                showLoading();
                try {
                    Export();
                    hideLoading();
                } catch (error) {
                    hideLoading();
                    showPopupError();
                }
            })

            function Export() {
                if (sessionStorage.getItem("IsCefShap") == null) {
                    let fromdate = formatStrYMD($("#todate").val());
                    let todate = formatStrYMD($("#todate").val());
                    let BranchID = sessionStorage.getItem("BranchId");
                    let sqlparam = `@Action = 'GET' , @FromDate = '${fromdate}' ,@ToDate = '${todate}' ,@UserName = '${localStorage.getItem('username1')}' `;
                    var url = `/api/sqlQuerryBCTD/BaoCaoTienDo_XuatExcel?branch=${BranchID}&storeName=SP_ERP_BAOCAOTIENDOSXREPORT&sqlPara=${sqlparam}`
                    var link = document.createElement('a');
                    var filename = `BaoCaoTongHopTienDoSanXuat.xlsx`;
                    link.href = url;
                    link.download = filename;
                    link.style.display = 'none';
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                } else {
                    let fromdate = formatStrYMD($("#todate").val());
                    let todate = formatStrYMD($("#todate").val());
                    let BranchID = sessionStorage.getItem("BranchId");
                    let sqlparam = `@Action = 'GET' , @FromDate = '${fromdate}' ,@ToDate = '${todate}' ,@UserName = '${localStorage.getItem('username1')}' `;
                    //var url = `/api/sqlQuerryBCTD/BaoCaoTienDo_XuatExcel?branch=${BranchID}&storeName=SP_ERP_BAOCAOTIENDOSXREPORT&sqlPara=${sqlparam}`
                    let dataSend = {
                        BranchID: BranchID,
                        StoredName: 'SP_ERP_BAOCAOTIENDOSXREPORT',
                        SqlParam: sqlparam
                    };
                    let dataSendArr = [];
                    dataSendArr.push(dataSend);
                    console.log(`ExportExcel@${JSON.stringify(dataSendArr)}`);
                }
            }
            hideLoading();
        } catch (error) {
            hideLoading();
            showPopupError();
        }
    }
})
function showLoading() {
    $('#loading').show();
}

function hideLoading() {
    $('#loading').hide();
}

function Mergecell() {
    const previousRow = {};
    const colsChanged = {};
    const colsToMerge = [0, 1, 2, 3, 4];
    let leftMerged = false;
    let dark = false;

    $('.tbody tr').each(function (rowIdx, tr) {
        $(tr).children().each(function (colIdx, td) {
            const tdText = $(td).text();
            if (rowIdx > 0 && (colIdx === 0 || leftMerged) && previousRow[colIdx].text === tdText && colsToMerge.includes(colIdx)) {
                previousRow[colIdx].elem.attr('rowspan', ++previousRow[colIdx].span);
                colsChanged[colIdx] = false;
                $(td).remove();
                if (colIdx === 0) {
                    leftMerged = true;
                }
            } else {
                previousRow[colIdx] = { span: 1, text: tdText, elem: $(td), dark };
                colsChanged[colIdx] = true;
            }
        });

        const rowChanged = Object.values(colsChanged).every(Boolean);
        dark = rowChanged && rowIdx > 0 ? !dark : dark;
        if (dark) {
            $(tr).addClass('dark');
        }
        leftMerged = false;
    });
}


// Gọi hàm để bắt đầu kiểm tra
