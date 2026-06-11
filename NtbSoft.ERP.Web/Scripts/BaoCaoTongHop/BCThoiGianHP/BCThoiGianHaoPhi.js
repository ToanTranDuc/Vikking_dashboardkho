
var DepID_fisrt = "";
var fromDate, toDate = "";
var DepID = "";
var countRed = 0, countYellow = 0, countGreen = 0, countBlue = 0, TimesRed = 0, TimesYellow = 0, TimesGreen = 0, TimesBlue = 0;
let chart; // define chart variable outside of function
let chart1;
var bieudobamden = []
var bieudothoigianhaophi = []
function checknull(value) {
    if (value == null) {
        return "";
    }
    return value;
}

function SetValue(value) {
    if (value == null) {
        return 0;
    }
    return value;
}

function loadPage(DepID, fromDate, toDate) {
    getDataReport(DepID, fromDate, toDate)
        .then(data => {
            if (data.length > 0) {
                let html = "";
                countRed = 0, countYellow = 0, countGreen = 0, countBlue = 0, TimesRed = 0, TimesYellow = 0, TimesGreen = 0, TimesBlue = 0;
                data.map((x, index) => {
                    html += `
                    <tr>
                    <td class="center">${index + 1}</td>
                    <td>${x.CreatedDate}</td>
                    <td class="center">${x.LightID}</td>
                    <td>${checknull(x.RedBegin)}</td>
                    <td>${checknull(x.RedStart)}</td>
                    <td>${checknull(x.RedEnd)}</td>
                    <td class="right">${checknull(x.RedTotalSeconds)}</td>
                    <td>${checknull(x.YellowBegin)}</td>
                    <td>${checknull(x.YellowStart)}</td>
                    <td>${checknull(x.YellowEnd)}</td>
                    <td class="right">${checknull(x.YellowTotalSeconds)}</td>
                    <td>${checknull(x.GreenBegin)}</td>
                    <td>${checknull(x.GreenStart)}</td>
                    <td>${checknull(x.GreenEnd)}</td>
                    <td class="right">${checknull(x.GreenTotalSeconds)}</td>
                    <td>${checknull(x.BlueBegin)}</td>
                    <td>${checknull(x.BlueStart)}</td>
                    <td>${checknull(x.BlueEnd)}</td>
                    <td class="right">${checknull(x.BlueTotalSeconds)}</td>
                    </tr>
                    `

                    if (SetValue(x.RedTotalSeconds) > 0) {

                        countRed++;
                        TimesRed += parseInt(SetValue(x.RedTotalSeconds));
                    }
                    if (SetValue(x.YellowTotalSeconds) > 0) {
                        countYellow++;
                        TimesYellow += parseInt(SetValue(x.YellowTotalSeconds));
                    }
                    if (SetValue(x.GreenTotalSeconds) > 0) {
                        countGreen++;
                        TimesGreen += parseInt(SetValue(x.GreenTotalSeconds));
                    }
                    if (SetValue(x.BlueTotalSeconds) > 0) {
                        countBlue++;
                        TimesBlue += parseInt(SetValue(x.BlueTotalSeconds));
                    }

                })
                $("#tbodyContent").html(html);
                console.log(countRed, "--", countYellow, "--", countGreen, "--", countBlue, "--", TimesRed, "--", TimesYellow, "--", TimesGreen, "--", TimesBlue)

                let bieudothoigianhaophi = [{
                    name: 'Cần thợ máy',
                    y: TimesRed
                }, {
                    name: 'Cần tổ trưởng',
                    y: TimesYellow
                }, {
                    name: 'Cần kỹ thuật',
                    y: TimesGreen
                }, {
                    name: 'Công nhân ra ngoài',
                    y: TimesBlue
                }]

                let bieudobamden = [{
                    name: 'Cần thợ máy',
                    y: countRed
                }, {
                    name: 'Cần tổ trưởng',
                    y: countYellow
                }, {
                    name: 'Cần kỹ thuật',
                    y: countGreen
                }, {
                    name: 'Công nhân ra ngoài',
                    y: countBlue
                }]
                renderT(bieudobamden, "pieChart", "Biểu Đồ Số Lần Bấm Đèn")
                renderT(bieudothoigianhaophi, "pieChart1", "Biểu Đồ Thời Gian Hao Phí")
                $("#sum_tgxl1").html(TimesRed)
                $("#sum_tgxl2").html(TimesYellow)
                $("#sum_tgxl3").html(TimesGreen)
                $("#sum_tgxl4").html(TimesBlue)
            }
            else {
                countRed = 0, countYellow = 0, countGreen = 0, countBlue = 0, TimesRed = 0, TimesYellow = 0, TimesGreen = 0, TimesBlue = 0;
                let bieudothoigianhaophi = [{
                    name: 'Cần thợ máy',
                    y: TimesRed
                }, {
                    name: 'Cần tổ trưởng',
                    y: TimesYellow
                }, {
                    name: 'Cần kỹ thuật',
                    y: TimesGreen
                }, {
                    name: 'Công nhân ra ngoài',
                    y: TimesBlue
                }]

                let bieudobamden = [{
                    name: 'Cần thợ máy',
                    y: countRed
                }, {
                    name: 'Cần tổ trưởng',
                    y: countYellow
                }, {
                    name: 'Cần kỹ thuật',
                    y: countGreen
                }, {
                    name: 'Công nhân ra ngoài',
                    y: countBlue
                }]
                renderT(bieudobamden, "pieChart", "Biểu Đồ Số Lần Bấm Đèn")
                renderT(bieudothoigianhaophi, "pieChart1", "Biểu Đồ Thời Gian Hao Phí")

                let html = "";
                $("#tbodyContent").html(html);
                $("#sum_tgxl1").html(0)
                $("#sum_tgxl2").html(0)
                $("#sum_tgxl3").html(0)
                $("#sum_tgxl4").html(0)
            }
        })
        .catch(error => {
            console.error("Failed to load data: ", error);
        });



}

async function GetLine(userLogin) {

    //  WHERE (''${userLogin}'' = ul.UserID OR ''${userLogin}'' = ''admin'')
    const Para = `@sqlParam='SELECT DISTINCT stl.ID,stl.Name ,stl.IDSub,stl.Sort,stl.GroupID,stl.TimeWorkID,stl.GroupLightID,
                   stl.Rate_Red,stl.Rate_Yellow,stl.Rate_Green,stl.BTP_Red,stl.BTP_Yellow,stl.BTP_Green,
                   stl.IsTerminal,stl.Id_Factory,stl.ReadAt,CONVERT(BIT, 0) as IsPlay, SerializeTimeShiftCollection, TotalTimeTW, 0 as TimeCurrent
                   FROM dbo.tbl_SettingLineX stl WITH (NOLOCK)
		           LEFT JOIN dbo.SYS_USER_LINE ul WITH (NOLOCK) ON	stl.ID = ul.LineID
                   left join tbl_TimeWorks tw on stl.TimeWorkID = tw.ID
                   WHERE (''${userLogin}'' = ul.UserID OR ''${userLogin}'' = ''admin'')
                   order by Sort asc'`
    const data = await ExecProc("ChayTT", Para)
    DepID_fisrt = data[0]["ID"];
    let html = "";
    data.map((x, index) => {
        html += `
                    <option value="${x.ID}">${x.Name}</option>
                    `
    })
    $("#slc_lsx").html(html)

    return DepID_fisrt;
}

function renderT(arr, idElement, title) {
    var colors = ['red', 'yellow', 'green', 'blue'];
    Highcharts.chart(`${idElement}`, {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie',
        },
        title: {
            text: title,
            align: 'center'
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.2f}%</b>'
        },
        accessibility: {
            point: {
                valueSuffix: '%'
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    //format: '{point.percentage:.2f} %' //'{point.percentage:.1f} %'
                    format: idElement == "pieChart" ? '{point.y}' : '{point.percentage:.2f} %'
                },
                showInLegend: true
            }
        },
        series: [{
            name: 'Brands',
            colorByPoint: true,
            colors: colors,
            data: arr
        }]
    });

}
/*event func*/
async function doSomethingAsync() {
    try {
        fromDate = $(".tuNgay").eq(0).val();
        toDate = $(".denNgay").eq(0).val();
        await GetLine('admin');
        await loadPage(DepID_fisrt, formatStrYMD(fromDate), formatStrYMD(toDate));
    }
    catch (error) {
        // If it does we will catch the error here.
    }
}

$("#slc_lsx").on("change", function () {
    fromDate = $(".tuNgay").eq(0).val();
    toDate = $(".denNgay").eq(0).val();
    loadPage($(this).val(), formatStrYMD(fromDate), formatStrYMD(toDate));
})

$("#btnFind").on("click", function () {
    DepID = $("#slc_lsx").val();
    fromDate = $(".tuNgay").eq(0).val();
    toDate = $(".denNgay").eq(0).val();
    loadPage(DepID, formatStrYMD(fromDate), formatStrYMD(toDate));
});

/*func execute sql*/

async function ExecProc(spName, Para) {
    try {
        let BranchID = sessionStorage.getItem("BranchId");
        let response = await RunStored_GetData(BranchID, spName, Para);
        return response;
    } catch (error) {
        console.error("Error executing stored procedure:", error);
        throw error; // Handle the error or rethrow it
    }
}

/*func get data*/
function getDataReport(DepID, fromDate, toDate) {
    return new Promise(async (resolve, reject) => {
        try {

            var arrSignalX = await ExecProc("ChayTT", `@sqlParam='select * from tbl_SignalX with(nolock) order by SignalX asc'`);

            setTimeout(async () => {
                let SqlSignalX1 = '';
                arrSignalX.forEach((item) => {
                    SqlSignalX1 += `select ROW_NUMBER() over ( partition by ${item.SignalName}CreatedDate,GroupLightID${item.SignalName},${item.SignalName}Signal
                                                        order by ${item.SignalName}CreatedDate,GroupLightID${item.SignalName},${item.SignalName}Signal asc
                                                        ) Pid${item.SignalName},*

                                                        into #A${item.SignalX}

                                                        From (select CONVERT(date,CreatedDate,121) as ${item.SignalName}CreatedDate
                                                                ,A.LightID as GroupLightID${item.SignalName}
                                                                ,TimeBegin as ${item.SignalName}Begin
                                                                ,TimeStart as ${item.SignalName}Start
                                                                ,TimeEnd as ${item.SignalName}End
                                                                ,SignalX as ${item.SignalName}Signal
                                                                ,TotalSeconds as ${item.SignalName}TotalSeconds
                                                                From dbo.tbl_LineCurrents A with(nolock)
                                                                where   A.SignalX=${item.SignalX} and convert(date, A.CreatedDate,121) between ''${fromDate}'' and ''${toDate}'' and A.LineX=''${DepID}''
                                                        )main;`
                });

                let SqlSignalX2 = `
                    IF OBJECT_ID(''tempdb.dbo.#AG'', ''U'') IS NOT NULL DROP TABLE #AG;
                    IF OBJECT_ID(''tempdb.dbo.#A0'', ''U'') IS NOT NULL DROP TABLE #A0;
                    IF OBJECT_ID(''tempdb.dbo.#A1'', ''U'') IS NOT NULL DROP TABLE #A1;
                    IF OBJECT_ID(''tempdb.dbo.#A2'', ''U'') IS NOT NULL DROP TABLE #A2;
                    IF OBJECT_ID(''tempdb.dbo.#A3'', ''U'') IS NOT NULL DROP TABLE #A3;

                    select
                        ROW_NUMBER() over ( partition by CreatedDate,LineX,LightID,SignalX
                        order by CreatedDate,LineX,LightID,SignalX  asc ) Pid,*

                    into #AG
                    from( select convert(date,CreatedDate,121) as CreatedDate,LineX,SignalX, LightID,AutoID
                        From dbo.tbl_LineCurrents A with(nolock)
                        where convert(date, A.CreatedDate,121) between ''${fromDate}'' and ''${toDate}'' and A.LineX=''${DepID}''
                        group by CreatedDate,LineX, LightID,SignalX,AutoID )main;
                    ${SqlSignalX1}
                `;

                let sql = `
                    @sqlParam=N'${SqlSignalX2}
                    select distinct 0 as STT, A1.Pid, CONVERT(varchar(10), A1.CreatedDate,103) as CreatedDate,LineX,LightID,a.*,b.*,c.*,d.*
                    from #AG A1
                    left join #A0 a on A1.LightID = a.GroupLightIDRed and A1.CreatedDate=A.RedCreatedDate and A1.Pid=PidRed
                    left join #A1 b on A1.LightID = b.GroupLightIDYellow and A1.CreatedDate=b.YellowCreatedDate and A1.Pid=PidYellow
                    left join #A2 c on A1.LightID = c.GroupLightIDGreen and A1.CreatedDate=c.GreenCreatedDate and A1.Pid=PidGreen
                    left join #A3 d on A1.LightID = d.GroupLightIDBlue and A1.CreatedDate=d.BlueCreatedDate and  A1.Pid=PidBlue
                    order by CreatedDate, RedBegin, YellowBegin, GreenBegin, BlueBegin asc;

                    drop table #AG;
                    drop table #A0;
                    drop table #A1;
                    drop table #A2;
                    drop table #A3;
                '`;


                var arrResult = await ExecProc("ChayTT", sql);
                resolve(!arrResult || arrResult.length === 0 ? [] : arrResult);

            }, 1000);
        } catch (error) {

            reject(error);
        }
    });
}

$('#btnXuat_Excel').on('click', function () {
    let data_lst = [];
    $('#tbodyContent tr').each(function () {
        let STT = $(this).find('td').eq(0).text();
        let Ngay = $(this).find('td').eq(1).text();
        let Nhom = $(this).find('td').eq(2).text();
        let CTM_BatDauBatDen = $(this).find('td').eq(3).text();
        let CTM_BatDauXuLi = $(this).find('td').eq(4).text();
        let CTM_KetThuc = $(this).find('td').eq(5).text();
        let CTM_ThoiGianXuLi = $(this).find('td').eq(6).text();

        let CKT_BatDauBatDen = $(this).find('td').eq(7).text();
        let CKT_BatDauXuLi = $(this).find('td').eq(8).text();
        let CKT_KetThuc = $(this).find('td').eq(9).text();
        let CKT_ThoiGianXuLi = $(this).find('td').eq(10).text();

        let CNRN_BatDauBatDen = $(this).find('td').eq(11).text();
        let CNRN_BatDauXuLi = $(this).find('td').eq(12).text();
        let CNRN_KetThuc = $(this).find('td').eq(13).text();
        let CNRN_ThoiGianXuLi = $(this).find('td').eq(14).text();

        let data_item = {
            STT: STT,
            Ngay: Ngay,
            Nhom: Nhom,
            CTM_BatDauBatDen: CTM_BatDauBatDen,
            CTM_BatDauXuLi: CTM_BatDauXuLi,
            CTM_KetThuc: CTM_KetThuc,
            CTM_ThoiGianXuLi: CTM_ThoiGianXuLi,

            CKT_BatDauBatDen: CKT_BatDauBatDen,
            CKT_BatDauXuLi: CKT_BatDauXuLi,
            CKT_KetThuc: CKT_KetThuc,
            CKT_ThoiGianXuLi: CKT_ThoiGianXuLi,

            CNRN_BatDauBatDen: CNRN_BatDauBatDen,
            CNRN_BatDauXuLi: CNRN_BatDauXuLi,
            CNRN_KetThuc: CNRN_KetThuc,
            CNRN_ThoiGianXuLi: CNRN_ThoiGianXuLi
        };
        data_lst.push(data_item);
    });

    if (data_lst.length == 0) {
        Swal.fire({
            title: 'Thông báo',
            text: 'Không có dữ liệu ở thời gian được chọn để xuất!',
            icon: 'info',
            confirmButtonText: 'OK'
        });
        return;
    }

    let url = `/api/XuatExcelBaoCaoThoiGianHaoPhi/ExportExcel`;
    let chuyenID = $("#slc_lsx").val();
    let chuyenName;
    $("#slc_lsx").find('option').each(function () {
        if ($(this).attr('value') == chuyenID) {
            chuyenName = $(this).text();
        }
    })
    let dataToSend = JSON.stringify({
        Chuyen: chuyenName,
        TuNgay: fromDate,
        DenNgay: toDate,
        DataBC: data_lst,
    });
    if (sessionStorage.getItem("IsCefShap") == null) {
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: dataToSend,
        })
            .then(response => {
                if (!response.ok) {
                    Swal.fire({
                        title: 'Lỗi',
                        text: 'Xuất file báo cáo thất bại!',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                    return;
                }
                return response.blob();
            })
            .then(blob => {
                var a = document.createElement("a");
                var url = window.URL.createObjectURL(blob);
                a.href = url;
                a.download = 'BaoNKThanhPham.xlsx';
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
            });
    } else {
        console.log(`ExportExcel@${dataToSend}`);
    }
});

$(document).ready(async function () {
    localStorage.removeItem("ajax_data-1-meta");
    if (sessionStorage.getItem("BranchId") == null) { }
    else {
        if (sessionStorage.getItem("IsCefShap") != null)
            $('#BarMenu').hide();
        await doSomethingAsync();
    }
});

