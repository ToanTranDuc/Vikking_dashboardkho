
var selectedOption1 = "";
var selectedOption2 = "";


/*Init Date and varrible*/
var currentDate = new Date();


var formattedDate = currentDate.getFullYear() + '-' + ((currentDate.getMonth() + 1) < 10 ? '0' : '') + (currentDate.getMonth() + 1) + '-' + (currentDate.getDate() < 10 ? '0' : '') + currentDate.getDate();

/*let length = 100000;*/

/*Func Init*/

function getDateTimeNow() {
    var currentDate = new Date();
    var day = currentDate.getDate();
    var month = currentDate.getMonth() + 1;
    var year = currentDate.getFullYear();
    var hours = currentDate.getHours();
    var minutes = currentDate.getMinutes();
    var seconds = currentDate.getSeconds();


    day = day < 10 ? '0' + day : day;
    month = month < 10 ? '0' + month : month;
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;


    var formattedDateTime = day + '-' + month + '-' + year + '-' + hours + minutes + seconds;
    return formattedDateTime;
}


function handleDate(_date) {
    var date = new Date(_date);
    return date.getFullYear() + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate()));
}

/*func call api */



function getTable() {
    const Para = `@Action='GetDetail',@Ngay='${formattedDate}',@LineX='',@fromDate='',@toDate='',@year=''`;
    const d = ExecProc("SP_SEW_NHANLUC", Para)
    charTable1(d)
}


function getSL(date) {

    const Para = `@Action='GetSL',@Ngay='${formatStrYMD(date)}',@LineX='',@fromDate='',@toDate='',@year=''`;
    const d = ExecProc("SP_SEW_NHANLUC", Para)
    gethighChar(d)


}

function getSLAll(frDate, toDate) {
    const Para = `@Action='GetSLAll',@Ngay='',@LineX='',@fromDate='${formatStrYMD(frDate)}',@toDate='${formatStrYMD(toDate)}',@year='' `;
    const d = ExecProc("SP_SEW_NHANLUC", Para)
    fechChar(d)
}

let getYear = function () {
    const Para = `@Action='GetYear',@Ngay='',@LineX='',@fromDate='',@toDate='',@year=''`;
    const d = ExecProc("SP_SEW_NHANLUC", Para)
    personnel(d)
    myChart(d)
}

function getMonth(year) {
    const Para = `@Action='GetMonth',@Ngay='',@LineX='',@fromDate='',@toDate='',@year='${year}'`;
    const d = ExecProc("SP_SEW_NHANLUC", Para);
    personnel1(d)
    myChart1(d)

}

function getDate(date) {
    const Para = `@Action='GetNgay',@Ngay='${formatStrYMD(date)}',@LineX='',@fromDate='',@toDate='',@year=''`;
    const d = ExecProc("SP_SEW_NHANLUC", Para);
    charTable1(d)
    renderTableSl(d)
}
/*func orther*/

let charTable1 = function (d) {
    let a = []
    let b = []
    let c = []
    let e = []
    if (d) {
        d.map(x => {
            a.push(x.SoLaoDong)
            b.push(x.SoGioLam)
            c.push(x.SoLaoDongVang)
            e.push(x.DepID)

        })

    }

    Highcharts.chart('container7', {
        chart: {
            type: 'column'
        },
        xAxis: {
            categories: e
        },
        plotOptions: {
            column: {
                grouping: false,
                shadow: false,
                borderWidth: 0, dataLabels: {
                    enabled: true,
                    format: '{point.y}'
                }
            }
        },
        series: [
            {
                name: 'Số lao động vắng',
                color: 'red',
                data: c,
                pointPadding: 0.4,
                pointPlacement: -0.2
            }
        ],
        yAxis: [
            {
                min: 0,
                title: {
                    text: ''
                }
            },

        ]
    });
}



let personnel = function (d) {
    let html = `<option value="ALL Year">Tất cả</option>`;
    let a = [];
    let b = [];

    if (d) {
        d.map(x => {
            a.push(x.Nam)
            b.push(x.SLD)
        })

        for (var i = 0; i < d.length; i++) {
            html += `
                <option value=${d[i].Nam}>${d[i].Nam}</option>
                `
            $("#year").html(html)
        }
    }

    Highcharts.chart('container8', {

        chart: {
            type: 'column'
        },

        title: {
            text: '',
            align: 'left'
        },

        xAxis: {
            categories: a
        },

        yAxis: {
            allowDecimals: false,
            min: 0,
            title: {
                text: ''
            }
        },

        tooltip: {
            format: '<b>{key}</b><br/>{series.name}: {y}<br/>' +
                'Total: {point.stackTotal}'
        },

        plotOptions: {
            column: {
                stacking: 'normal',
                dataLabels: {
                    enabled: true,
                    format: '{point.y}'
                }
            }
        },

        series: [{
            name: 'Lao động',
            data: b,
            stack: 'Europe'
        },]
    });

}

let personnel1 = function (d) {


    let a = [];
    let b = [];
    if (d) {
        d.map(x => {
            a.push("Tháng" + x.thang);
            b.push(x.SLD)
        })

    }

    Highcharts.chart('container10', {

        chart: {
            type: 'column'
        },

        title: {
            text: 'Olympic Games all-time medal table, grouped by continent',
            align: 'left'
        },

        xAxis: {
            categories: a
        },

        yAxis: {
            allowDecimals: false,
            min: 0,
            title: {
                text: ''
            }
        },

        tooltip: {
            format: '<b>{key}</b><br/>{series.name}: {y}<br/>' +
                'Total: {point.stackTotal}'
        },

        plotOptions: {
            column: {
                stacking: 'normal',
                dataLabels: {
                    enabled: true,
                    format: '{point.y}'
                }
            }
        },

        series: [{
            name: 'Lao động',
            data: b,
            stack: 'Europe'
        },]
    });

}

let renderTableSl = function (d) {
    if (!d) return;

    let renderNotNull = d.filter(function (item) {
        return item.SoLaoDong !== 0;
    });
    let Total_Worker = 0;
    let html = '';
    for (var i = 0; i < renderNotNull.length; i++) {
        html += `<tr>
                    <td>${renderNotNull[i].DepID}</td>
                    <td>${renderNotNull[i].SoLaoDong}</td>
                 </tr>`;

        Total_Worker += renderNotNull[i].SoLaoDong;
    }
    $("#bodyTable").html(html);
    $(".tfoot-total-worker").html(Total_Worker)

    if (d) {
        $(".pass").show();
    } else {
        $(".pass").hide();
    }
};


let fechChar = function (d) {
    let a = [];
    let b = [];
    let c = []
    if (d == undefined) {

    } else {
        d.map(x => {
            a.push(x.DepID)
            b.push(x.TongLD)
            c.push(x.LDV)
        })
    }
    console.log(a, b, c, "bbbnn")
    Highcharts.chart('container4', {
        chart: {
            type: 'column'
        },
        title: {
            text: 'Biểu đồ cột với dữ liệu chồng lên và số liệu trên cột'
        },
        xAxis: {
            categories: a
        },
        yAxis: {
            title: {
                text: ''
            }
        },
        plotOptions: {
            column: {
                stacking: 'normal',
                dataLabels: {
                    enabled: true,
                    format: '{point.y}'
                }
            }
        },
        series: [{
            name: 'Lao động vắng',
            data: c,
            stack: 'stack1'
        }, {
            name: 'Lao động',
            data: b,
            stack: 'stack1'
        }]
    });
}

let gethighChar = function (d) {

    let filteredArray = []

    if (d) {
        let dataObject = d[0];
        filteredArray = Object.entries(dataObject)
            .filter(([key, value]) => value !== null)
            .map(([key, value], i) => ({
                name: 'Chuyền ' + key,
                y: parseFloat(value)
            }));

        console.log(filteredArray);
    }
    Highcharts.chart('container6', {

        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: '',
            align: 'left'
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.y}</b>'
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
                    format: '<b>{point.name}</b>: {point.y}',
                    connectorColor: 'silver'
                }
            }
        },
        series: [{
            name: 'Lao động',
            data: filteredArray
        }]
    });


}

let myChart = function (d) {
    let arrCategories = [];
    let b = [];
    let c = [];
    if (d) {
        d.map(x => {
            arrCategories.push(x.Nam)
            b.push(x.SoLaoDongThem)
            c.push(x.SoLaoDongNghiViec)
        })
    }

    Highcharts.chart('container9', {
        title: {
            text: ''
        },
        xAxis: {
            categories: arrCategories
        },
        yAxis: {
            title: {
                text: ''
            }
        },
        series: [{
            name: 'Lao đông tăng',
            type: 'line',
            data: b
        }, {
            name: 'Lao động giảm ',
            type: 'line',
            data: c
        }]
    });


}

let myChart1 = function (d) {
    let a = [];
    let b = [];
    let c = [];
    if (d) {
        d.map(x => {
            a.push('Tháng ' + x.thang)
            b.push(x.SoLaoDongThem)
            c.push(x.SoLaoDongNghiViec)
        })
    }
    const labels = a;
    const data1 = b;
    const data2 = c;

    Highcharts.chart('container11', {
        title: {
            text: ''
        },
        xAxis: {
            categories: labels
        },
        yAxis: {
            title: {
                text: ''
            }
        },
        series: [{
            name: 'Lao động tăng',
            type: 'line',
            data: data1
        }, {
            name: 'Lao động giảm',
            type: 'line',
            data: data2
        }]
    });
}

/*event func*/

$("#date").on("change", function (e) {
    let keyup = $(this).val();
    getDate(keyup)
    getSL(keyup)


});

$("#chuyen2").on('change ', function () {
    selectedOption1 = $(this).find("option:selected").text().replace("Chuyền", "");
    selectedOption2 = $(this).find("option:selected").text();
    $("#text3").val(selectedOption2);
});

$("#click-me").on("click", function () {
    var fromDate = $("#fdate").val();
    var toDate = $("#tdate").val();

    if (fromDate > toDate) {
        alert("Xin chọn lại ngày")
        return;

    }
    else {
        if (fromDate && toDate) {
            /* getFromTo(fromDate, toDate, '01_CG')*/
            getSLAll(fromDate, toDate)

        }
    }
})

$("#year").on('change ', function () {
    selectedOption2 = $(this).find("option:selected").val()
    if (!selectedOption2) return;
    if (selectedOption2 === 'ALL Year') {
        $("#container8").show()
        $("#container10").hide()
        $("#container9").show()
        $("#container11").hide()
    } else {
        getMonth(selectedOption2)
        $("#container8").hide()
        $("#container10").show()
        $("#container9").hide()
        $("#container11").show()
    }
});

/*excute proc*/

function ExecProc(spName, Para) {
    try {
        let BranchID = sessionStorage.getItem("BranchId");
        const response = RunStored_GetData(BranchID, spName, Para);
        return response;
    } catch (error) {
        console.error("Error executing stored procedure:", error);
        throw error;
    }
}


$("#btn-excel").on("click", function () {
    let BranchID = sessionStorage.getItem("BranchId");
    const objPara = {
        BrandID: BranchID,
        LaborOfDate: formatStrYMD($("#date").val()),
        Option: $("#year").val(),
        FromDate: formatStrYMD($("#fdate").val()),
        ToDate: formatStrYMD($("#tdate").val())
    };


    var url = `/api/BCNhanLuc/ExportExcel`;
    var fileName = `Báo Cáo Nhân Lực ${getDateTimeNow()}`
    if (sessionStorage.getItem("IsCefShap") == null) {
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(objPara),
        })
            .then(response => {
                if (!response.ok) {
                    //Toast.fire({
                    //    icon: 'error',
                    //    title: 'Lỗi Không thể xuất excel . Vui lòng thử lại!'
                    //})
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
    }
    else {
        console.log(`ExportExcel@${JSON.stringify(objPara)}`);
    }

})



/**/
$(document).ready(function () {

    localStorage.removeItem("ajax_data-1-meta");
    if (sessionStorage.getItem("BranchId") == null) { }
    else {
        if (sessionStorage.getItem("IsCefShap") != null)
            $('#BarMenu').hide();
        getSL(formattedDate)
        getTable()
        getYear()
        getSLAll(formattedDate, formattedDate)
    }

});

