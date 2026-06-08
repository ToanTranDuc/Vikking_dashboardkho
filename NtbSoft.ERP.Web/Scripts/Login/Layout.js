$(document).ready(function () {
    var IsLogin = localStorage.getItem('IsLogin');
    if (IsLogin != "1") {
        window.location = "/Login/Index";
    }
    GetUrl();
    CheckUserPermission();
});
function GetUrl() {
    $.ajax({
        type: "Get",
        url: "/api/SystemUser/GetUrl",
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (d) {
            d.map((x, index) => {
                try {
                    const ModuleId = document.querySelector('#' + x.Key);
                    ModuleId.href = x.Value;
                }
                catch {

                }
               
            })
           
        },
        error: function (errormessage) {
            toast({
                title: "Thất bại",
                message: errormessage.responseJSON.ExceptionMessage,
                type: "error",
                duration: 3000
            });
        }
    });
}
function CheckUserPermission() {
    var User = localStorage.getItem('UserName');
    $('#UserLogin').html("  " + User);
    var lstModule = localStorage.getItem("ModuleQTY");
    var ojData = JSON.parse(lstModule);
    var lstNhomModule = [...new Set(ojData.map(item => item.NhomModule))];
    $('#Cat').css("display", "none");
    $('#HoanThanh').css("display", "none");
    lstNhomModule.map((x, index) => {
        if (x == "1") $('#Cat').css("display", "")
        else if (x == "2") $('#HoanThanh').css("display", "")
    });
   
    $('#NhanTP').css("display", "none");
    $('#QuanLyCat').css("display", "none");
    $('#HachToanCat').css("display", "none");
    $('#KCSCat').css("display", "none");
    $('#KCSInTheu').css("display", "none");
    $('#DongThung').css("display", "none");
    $('#NhapKho').css("display", "none");
    $('#NKNhiemKL').css("display", "none");
    $('#KiemTraTPDown').css("display", "none");
    $('#GapXep').css("display", "none");


    $.each(ojData, function (key, item) {
        switch (parseInt(item.MaModule)) {
            case 1:
                $('#Cat').css("display", "");
                $('#HoanThanh').css("display", "");
                $('#NhanTP').css("display", "");
                $('#QuanLyCat').css("display", "");
                $('#HachToanCat').css("display", "");
                $('#KCSCat').css("display", "");
                $('#KCSInTheu').css("display", "");
                $('#DongThung').css("display", "");
                $('#NhapKho').css("display", "");
                $('#NKNhiemKL').css("display", "");
                $('#KiemTraTPDown').css("display", "");
		$('#GapXep').css("display", "");
                break;
            case 2:
                $('#NhanTP').css("display", "");
                break;
            case 3:
                $('#QuanLyCat').css("display", "");
                break;
            case 4:
                $('#HachToanCat').css("display", "");
                break;
            case 5:
                $('#KCSCat').css("display", "");
                break;
            case 6:
                $('#KCSInTheu').css("display", "");
                break;
            case 7:
                $('#DongThung').css("display", "");
                break;
            case 8:
                $('#NhapKho').css("display", "");
                break;
            case 9:
                $('#NKNhiemKL').css("display", "");
                break;
            case 10:
                $('#KiemTraTPDown').css("display", "");
                break;
	    case 11:
                $('#GapXep').css("display", "");
                break;
        }
    });

}
