$(function () {
    function GetData() {
        let ndung = sessionStorage.getItem('UserNameLogging');
        let url = `/api/XuatExcelBaoCaoTienDoTongHop/Get?userName=${ndung}`;
        $.ajax({
            url: url,
            type: 'get',
            success: function (data) {
                $(".tableTongHop").empty()
                let data1 = data.dt
                let data2 = data.dt2
                let data3 = data.dt3
                let data4 = data.dt4
                let arrText = ["Nhà máy Tân Hương 1", "Nhà máy Tân Hương 2", "Nhà máy Chợ Gạo", "Nhà máy Cai Lậy"]
                let dataArray = [data1, data2, data3, data4];
                let rowContainer = `
                    <div class="row-container" style="display: flex; align-items: center; justify-content: space-between; margin-bottom: -20px;">
                        <div class="valueNhaMay">${arrText[0]}</div>
                                            </div>`;

                
                $(".tableTongHop").append(rowContainer);

                for (var i = 0; i < 4; i++) {
                    let valueText = (i == 0) ? '' : arrText[i]; 
                    populateTable(dataArray[i], valueText, i + 1);
                }
		$('.row-container').eq(0).empty();
            },
        });
    }

    let tt2 = 0
    let tt3 = 0
    let tt9 = 0
    let tt11 = 0
    let tt12 = 0
    let tt13 = 0
    let tt14 = 0
    let tt15 = 0
    function populateTable(data, valueText, value) {
        let htmlthead = `
                <tr>
                    <th rowspan="2">Chuyền</th>
                    <th rowspan="2">T LĐ</th>
                    <th rowspan="2">LĐ có mặt</th>
                    <th rowspan="2">Mã hàng / Po</th>
                    <th rowspan="2">Khách hàng</th>
                    <th rowspan="2">Tên hàng</th>
                    <th rowspan="2">S.Lượng Kế hoạch</th>
                    <th rowspan="2">Ngày giao hàng</th>
                    <th colspan="4">Ra chuyền</th>
                    <th colspan="1">Kế Hoạch</th>
                    <th colspan="2">Chênh lệch so với mục tiêu</th>
                    <th rowspan="2">S.L.lũy kế</th>
                    <th rowspan="2">S.Lượng còn lại chưa may</th>
                    <th rowspan="2">Số ngày còn lại cần may</th>
                    <th colspan="2">DK ngày kết thúc may</th>
                    
                    <th rowspan="2">% DT T.hiện/DT mục tiêu</th>
                  
                </tr>
                <tr>
                    <th>S.L</th>
                    <th>Giá CM</th>
                    <th>
                        USD RC thực tế
                    </th>
                    <th>Usd/lđ/ngày</th>
                    <th>Mục tiêu</th>
                    <th>DT mục tiêu</th>
                    <th>Chênh lệch so với DT RC thực tế</th>
                    <th>Theo NS ngày báo cáo</th>
                   <th>Chênh lệch so với ngày giao hàng</th>
                </tr>
           
        `
        let htmlTbody = ` `
        let totalCol2 = 0, totalCol3 = 0, totalCol9 = 0, totalCol11 = 0, totalCol12 = 0, totalCol13 = 0, totalCol14 = 0, totalCol15 = 0;
        let SoNhaMay = 0;
console.log(data); 
        data.map(item => {
            // Tính tổng cho các cột
            totalCol2 += parseFloat(item.LaoDong) || 0;
            totalCol3 += parseFloat(item.LDCoMat) || 0;
            totalCol9 += parseFloat(item.RC_Ngay) || 0;
            totalCol13 += parseFloat(item.MucTieu) || 0;
            totalCol14 += parseFloat(item.DT_MucTieu) || 0;
            totalCol15 += parseFloat(item.DT_ChenhLech) || 0;

            // Ngày giao hàng và tính tổng cho cột 11
            let ngayGH = new Date(item.NgayGH);
            let rcNgay = parseFloat(item.RC_Ngay);
            let donGia = parseFloat(item.DonGia);
            let totalRC_DonGia = (rcNgay * donGia).toFixed(4);
            let cot11 = totalRC_DonGia.split('.');
            let cot11ne = (parseFloat(cot11[0]) + parseFloat('0.' + cot11[1]))
            totalCol11 += parseFloat(totalRC_DonGia) || 0;
			
            // Tính cột 12
            let ldCoMat = parseFloat(item.LDCoMat);
            let result = ldCoMat !== 0 ? (totalRC_DonGia / ldCoMat).toFixed(4) : '0.00';
            let cot12 = result.split('.');
            let cot12ne = ( parseFloat(cot12[0]) + parseFloat('0.' + cot12[1])).toFixed(4);
		 let cot1212 = cot12ne.split('.')
		let cot1212ne = ( parseFloat(cot12ne[0]) + parseFloat('0.' + cot12ne[1]));
            totalCol12 = totalCol3 !== 0 ? (totalCol11 / totalCol3).toFixed(4) : '0.00';

            totalCol12 = totalCol3 !== 0 ? (totalCol11 / totalCol3) : '0.00';

            // Ngày kết quả và chênh lệch ngày
            let fixedDate = new Date();
            let soNgayConLai = parseInt(item.SoNgayConLai);

            let ngayKetQua = new Date(fixedDate);
            ngayKetQua.setDate(ngayKetQua.getDate() + soNgayConLai);

            let daysDifference = Math.floor((ngayGH - ngayKetQua) / (1000 * 60 * 60 * 24));

            htmlTbody += `
        <tr>
            <td class="tdChuyen">${item.Name}</td>
            <td>${item.LaoDong}</td>
            <td>${item.LDCoMat}</td>
            <td>${item.MaHang}</td>
            <td>${item.KhachHang}</td>
            <td>${item.TenHang}</td>
            <td class="tdSLKH">${item.SLKH}</td>
            <td>${ngayGH.toLocaleDateString('en-GB')}</td>
            <td class="td12">${item.RC_Ngay}</td>
            <td >${item.DonGia}</td>
            <td >${cot11ne}</td>
            <td >${cot1212ne}</td>
            <td class="tdSLKH">${item.MucTieu}</td>
            <td>${item.DT_MucTieu}</td>
            <td>${item.DT_ChenhLech}</td>
            <td>${item.RC_LK}</td>
            <td>${(parseFloat(item.SLKH) - parseFloat(item.RC_LK))}</td>
            <td>${item.SoNgayConLai}</td>
            <td>${ngayKetQua.toLocaleDateString('en-GB')}</td>
            <td>${daysDifference}</td>
            <td></td>
        </tr>
    `;
        });
        tt2 += totalCol2
        tt3 += totalCol3
        tt9 += totalCol9
        tt11 += totalCol11
        tt12 += totalCol12
        tt13 += totalCol13
        tt14 += totalCol14
        tt15 += totalCol15
        let tfooot = `
    <tr>
        <td>Tổng:</td>
        <td>${totalCol2}</td>
        <td>${totalCol3}</td>
        <td colspan="5"></td>
        <td>${totalCol9}</td>
        <td></td>
        <td>${(totalCol11.toFixed(4).slice(-2) === '00')
                ? totalCol11.toFixed(2)
                : totalCol11.toFixed(4)
            }</td>
        <td>${((totalCol11 / totalCol3).toFixed(4).slice(-2) === '00')
                ? (totalCol11 / totalCol3).toFixed(2)
                : (totalCol11 / totalCol3).toFixed(4)
            }</td>
        <td>${totalCol13.toFixed(2)}</td>
        <td>${(totalCol14.toFixed(4).slice(-2) === '00')
                ? totalCol14.toFixed(2)
                : totalCol14.toFixed(4)
            }</td>
        <td>${(totalCol15.toFixed(4).slice(-2) === '00')
                ? totalCol15.toFixed(2)
                : totalCol15.toFixed(4)
            }</td>
        <td colspan="5"></td>
    <td>

  ${totalCol14 === 0 ? '0%' : ((totalCol11 / totalCol14) * 100).toFixed(4) + '%'}
</td>

                    </tr>
        `
        let tfootTong = `
          <tr>
    <td>${value == 2 ? "Tổng TH1+TH2" : "Tổng TGI:"}</td>
    <td>${tt2}</td>
    <td>${tt3}</td>
    <td colspan="5"></td>
    <td>${tt9}</td>
    <td></td>
    <td>${(tt11.toFixed(4).slice(-2) === '00')
                ? tt11.toFixed(2)
                : tt11.toFixed(4)
            }</td>
    <td>${((tt11 / tt3).toFixed(4).slice(-2) === '00')
                ? (tt11 / tt3).toFixed(2)
                : (tt11 / tt3).toFixed(4)
            }</td>
    <td>${(tt13.toFixed(4).slice(-2) === '00')
                ? tt13.toFixed(2)
                : tt13.toFixed(4)
            }</td>
    <td>${(tt14.toFixed(4).slice(-2) === '00')
                ? tt14.toFixed(2)
                : tt14.toFixed(4)
            }</td>
    <td>${(tt15.toFixed(4).slice(-2) === '00')
                ? tt15.toFixed(2)
                : tt15.toFixed(4)
    }</td>
    <td colspan="5"></td>
<td>

  ${tt14 === 0 ? '0%' : ((tt11 / tt14) * 100).toFixed(4) + '%'}
</td>


                    </tr>`
        let htmlTable = `
               <table>
                    <thead>${htmlthead} </thead>
                     <tbody>${htmlTbody}</tbody>
                      <tfoot class="meomaydoreamon">
                        ${tfooot}
                        ${value == 1 || value == 3 ? "" : tfootTong}
                    </tfoot>
                 </table>
                
            `

        let tonghop = `
            <div class="valueNhaMay">${valueText}
            

</div>
        ${htmlTable}
        `

        $(".tableTongHop").append(tonghop)
    }
    GetData()
})
$(function () {

    function populateTable(data) {
        console.log(data)
        let htmlthead = `
        
                <tr>
                    <th rowspan="2">Chuyền</th>
                    <th rowspan="2">T LĐ</th>
                    <th rowspan="2">LĐ có mặt</th>
                    <th rowspan="2">Mã hàng / Po</th>
                    <th rowspan="2">Khách hàng</th>
                    <th rowspan="2">Tên hàng</th>
                    <th rowspan="2">S.Lượng Kế hoạch</th>
                    <th rowspan="2">Ngày giao hàng</th>
                    <th colspan="4">Ra chuyền</th>
                    <th colspan="1">Kế Hoạch</th>
                    <th colspan="2">Chênh lệch so với mục tiêu</th>
                    <th rowspan="2">S.L.lũy kế</th>
                    <th rowspan="2">S.Lượng còn lại chưa may</th>
                    <th rowspan="2">Số ngày còn lại cần may</th>
                    <th colspan="1">DK ngày kết thúc may</th>
                    <th rowspan="2">% DT T.hiện/DT mục tiêu</th>
                </tr>
                <tr>
                    <th>S.L</th>
                    <th>Giá CM</th>
                    <th>
                        USD RC thực tế
                    </th>
                    <th>Usd/lđ/ngày</th>
                    <th>Mục tiêu</th>
                    <th>DT mục tiêu</th>
                    <th>Chênh lệch so với DT RC thực tế</th>
                    <th>Theo NS ngày báo cáo</th>
<th>Chênh lệch so với ngày giao hàng</th>
                </tr>
           
        `
        let htmlTbody = ` `
        let totalCol2 = 0, totalCol3 = 0, totalCol9 = 0, totalCol11 = 0, totalCol12 = 0, totalCol13 = 0, totalCol14 = 0, totalCol15 = 0;

        data.map(item => {
            // Tính tổng cho các cột
            totalCol2 += parseFloat(item.LaoDong) || 0;
            totalCol3 += parseFloat(item.LDCoMat) || 0;
            totalCol9 += parseFloat(item.RC_Ngay) || 0;
            totalCol13 += parseFloat(item.MucTieu) || 0;
            totalCol14 += parseFloat(item.DT_MucTieu) || 0;
            totalCol15 += parseFloat(item.DT_ChenhLech).toFixed(4) || 0;


            // Ngày giao hàng và tính tổng cho cột 11

            let ngayGH = new Date(item.NgayGH);
            let rcNgay = parseFloat(item.RC_Ngay);
            let donGia = parseFloat(item.DonGia);
            let totalRC_DonGia = (rcNgay + donGia).toFixed(4);
            totalCol11 += parseFloat(totalRC_DonGia).toFixed(4) || 0;

            // Tính cột 12
            let ldCoMat = parseFloat(item.LDCoMat);
            
            let fixedDate = new Date(2024, 5, 2);
            let soNgayConLai = parseInt(item.SoNgayConLai);

            let ngayKetQua = new Date(fixedDate);
            ngayKetQua.setDate(ngayKetQua.getDate() + soNgayConLai);


            htmlTbody += `
     <tr>
    <td>${item.Name}</td>
    <td>${item.LaoDong}</td>
    <td>${item.LDCoMat}</td>
    <td>${item.MaHang}</td>
    <td>${item.KhachHang}</td>
    <td>${item.TenHang}</td>
    <td>${item.SLKH}</td>
    <td>${ngayGH.toLocaleDateString('en-GB')}</td>
    <td>${item.RC_Ngay}</td>
    <td>${item.DonGia}</td>
    <td>${totalRC_DonGia}</td>
    <td>${hehe} </td >
    <td>${(item.MucTieu.toFixed(4).slice(-2) === '00')
                    ? item.MucTieu.toFixed(2)
                    : item.MucTieu.toFixed(4)
                }</td>
    <td>${(item.DT_MucTieu.toFixed(4).slice(-2) === '00')
                    ? item.DT_MucTieu.toFixed(2)
                    : item.DT_MucTieu.toFixed(4)
                }</td>
    <td>${(item.DT_ChenhLech.toFixed(4).slice(-2) === '00')
                    ? item.DT_ChenhLech.toFixed(2)
                    : item.DT_ChenhLech.toFixed(4)
                }</td>
    <td>${item.RC_LK}</td>
    <td>${(parseFloat(item.SLKH) - parseFloat(item.RC_LK)).toFixed(2)}</td>
    <td>${item.SoNgayConLai}</td>
    <td>${ngayKetQua.toLocaleDateString('en-GB')}</td>
    <td>${daysDifference}</td>
</tr>

    `;
        });

        let htmlTable = `
                <thead  >${htmlthead} </thead>
                <tbody>${htmlTbody}</tbody>
                <div>
                    <tfooter>${hmtl}</tfooter>
                 </div>
                
            `
        $(".tableTongHop").append(htmlTable)
    }

  
});


$(document).ready(function () {
    if (!window.CefSharp) {
        $('#btn-XuatBC-TH').on('click', function () {
            let ndung = sessionStorage.getItem('UserNameLogging');

            let url = `/api/XuatExcelBaoCaoTienDoTongHop/ExportExcel?userName=admin`;
            fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                // body: dataToSend,
            })
                .then(response => {

                    return response.blob();
                })
                .then(blob => {
                    var a = document.createElement("a");
                    var url = window.URL.createObjectURL(blob);
                    a.href = url;
                    a.download = 'BaoCaoTienDoTongHopCacKhu.xlsx';
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                });
        });
    } else {
        $('#btn-XuatBC-TH').on('click', function () {
            console.log("ExportExcel");
        });
    }
});


