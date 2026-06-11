$(document).ready(function () {
   
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
    $('#toggle-password').click(function () {
        var passwordField = $('#password');
        var fieldType = passwordField.attr('type');
        if (fieldType === 'password') {
            passwordField.attr('type', 'text');
            $(this).html('<i style="font-size:20px" class="fas fa-eye"></i>');
        } else {
            passwordField.attr('type', 'password');
            $(this).html('<i style="font-size:20px" class="fas fa-eye-slash"></i>');
        }
    });

    // Handle remember password functionality
    var remember = localStorage.getItem('remember');
    if (remember === 'true') {
        var username = localStorage.getItem('username');
        var password = localStorage.getItem('password');
        $('#username').val(username);
        $('#password').val(password);
        $('#remember').prop('checked', true);
    }

    $('#login-form').submit(function (e) {
        e.preventDefault();
        var username = $('#username').val();
        var password = $('#password').val();
        localStorage.setItem('username1', username);
        localStorage.setItem('password1', password);

        if ($('#remember').is(':checked')) {
            localStorage.setItem('remember', 'true');
            localStorage.setItem('username', username);
            localStorage.setItem('password', password);
        } else {
            localStorage.setItem('remember', 'false');
            localStorage.removeItem('username');
            localStorage.removeItem('password');
        }
       
        $.ajax({
            url: "/api/Login/Get?para1=" + username + "&para2=" + password,
            type: 'Get',
            success: function (d) {
                if (d == "true") {
                    window.location.href = "/Home/Dashboard"
                } else {
                    toastr.error('Sai mật khẩu');
                }
               
            }
        })

    });
});
