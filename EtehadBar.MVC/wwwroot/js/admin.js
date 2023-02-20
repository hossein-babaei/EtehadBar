function preloader() {
    $('#preloader').fadeToggle();
}
function parseValidator(t) {
    $(t).data('validator', null);
    $.validator.unobtrusive.parse($(t));
}
function goUp() {
    $('html, body').animate({
        scrollTop: 0
    }, 500);
}
function uiAlert(msg, status) {
    UIkit.notification.closeAll();
    UIkit.notification({ message: msg, status: status, pos: 'bottom-center', timeout: 10000 });
}
function submitForm(form) {
    if ($(form).valid() == true) {
        preloader();
        $(form).submit();
    }
}
function getdate() {
    var today = new Date();
    var h = today.getHours();
    var m = today.getMinutes();
    var s = today.getSeconds();
    if (s < 10) {
        s = "0" + s;
    }
    if (m < 10) {
        m = "0" + m;
    }
    if (h < 10) {
        h = "0" + h;
    }
    $(".main-clock").text(h + ":" + m + ":" + s);
    setTimeout(function () { getdate() }, 500);
}

function getNotifications() {
    $.post('/admin/admin-notifications', {}, function (r) {
        var heartbeat = '<span class="uk-position-absolute heartbeat"></span>';
        if (r.sum > 0) {
            $('#notification-icon > a').prepend(heartbeat);
        }
        $('#n-contact').text(r.contacts);
    }, 'json');
}

function numberWithCommas(n) {
    return n.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

function checkboxBoolean(checkbox) {
    $(checkbox).val($(checkbox).is(':checked'));
}

$(document).ready(function () {
    new StickySidebar('#sidebar', {
        containerSelector: '#main-content',
        innerWrapperSelector: '#sidebar-inner',
        topSpacing: 60,
        bottomSpacing: 0
    });
    getdate();
    getNotifications();
});

$(document).on('keyup', '.format-number', function () {
    var value = $(this).val();
    let parent = $(this).parent();
    if (!parent.hasClass('appended')) {
        parent.append('<div class="format-append uk-text-left uk-text-light uk-font-sm"></div>');
        parent.addClass('appended');
    }
    parent.find('.format-append').text(numberWithCommas(value));
});