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

function ignoreSpecialCharAndWhitespaceInSelect2(params, data) {
    if (params.term == '' || typeof (params.term) === 'undefined')
        return data;

    let term = params.term.split(' ');
    let text = data.text;

    let founded = 0;
    $.each(term, function (i, v) {
        if ($.trim(v) != '')
            if (text.indexOf(v) > -1) {
                founded++;
            }
    });

    if (founded == term.length)
        return data;
    else
        return null;
}

function getAllCostModal() {
    const modalId = '#all-cost-report-modal';
    $.post('/admin/get-all-cost-modal-lists', {}, function (r) {
        let datesOptions = '';
        $.each(r.dates, (i, v) => {
            datesOptions += `<option value="${v.startDate}|${v.endDate}">${v.title}</options>`;
        });

        let customerOptions = '';
        $.each(r.customers, (i, v) => {
            customerOptions += `<option value="${v.id}">${v.name}</options>`;
        });
        console.log(r.dates);
        $(`${modalId} select[name=date]`).html(datesOptions);
        $(`${modalId} select[name=customer]`).html(customerOptions);
    }, 'json');
    UIkit.modal(modalId).show();
}

function submitAllCostModal() {
    $('#all-cost-report-modal form').submit();
}

function getGeneralModal() {
    const modalId = '#general-report-modal';
    $.post('/admin/get-calendars-json', {}, function (r) {
        let options = '';
        $.each(r, (i, v) => {
            options += `<option value="${v.id}">${v.title}</options>`;
        });
        $(`${modalId} select`).html(options);
    }, 'json');
    UIkit.modal(modalId).show();
}

function submitGeneralModal() {
    $('#general-report-modal form').submit();
}

function getSlashedLoadFactor() {
    const modalId = '#slashed-load-factor-modal';
    $.post('/admin/get-calendars-json', {}, function (r) {
        let options = '';
        $.each(r, (i, v) => {
            options += `<option value="${v.id}">${v.title}</options>`;
        });
        $(`${modalId} select`).html(options);
    }, 'json');
    UIkit.modal(modalId).show();
}

function submitSlashedLoadFactor() {
    $('#slashed-load-factor-modal form').submit();
}

function getHasCapacityUnrealVehicles() {
    let calendarId = $('#slashed-load-factor-modal select[name=calendarId]').val();

    window.open(`/report/get-has-capacity-unreal-vehicles?calendarId=${calendarId}`, '_blank');
}

function getDriversPeriodicActivity() {
    const modalId = '#drivers-periodic-activity-modal';
    $.post('/admin/get-calendars-json', {}, function (r) {
        let options = '';
        $.each(r, (i, v) => {
            options += `<option value="${v.id}">${v.title}</option>`;
        });
        $(`${modalId} select`).html(options);
    }, 'json');
    UIkit.modal(modalId).show();
}

function submitDriversPeriodicActivity() {
    let fromCalendarId = $('#drivers-periodic-activity-modal select[name=fromCalendarId]').val(),
        toCalendarId = $('#drivers-periodic-activity-modal select[name=toCalendarId]').val();

    window.open(`/excel/drivers-periodic-activity?fromCalendarId=${fromCalendarId}&toCalendarId=${toCalendarId}`, '_blank');
}

async function getGovLoadFactor() {
    const modalId = '#gov-load-factor-companies-modal';
    preloader();
    await $.post('/admin/get-load-factor-company', {}, function (r) {
        let options = '';
        $.each(r, (i, v) => {
            options += `<option value="${v.id}">${v.title}</option>`;
        });
        $(`${modalId} select[name=companyId]`).html(options);
    }, 'json').then(() => {
        preloader();
        UIkit.modal(modalId).show();
    });
}

$(document).ready(function () {
    $(`#general-report-modal select[name=calendarId]`).select2({ width: '100%' });
    $(`#all-cost-report-modal select[name=date]`).select2({ width: '100%' });
    $(`#all-cost-report-modal select[name=customer]`).select2({ width: '100%' });
    $(`#slashed-load-factor-modal select[name=calendarId]`).select2({ width: '100%' });
    $(`#drivers-periodic-activity-modal select`).select2({ width: '100%' });
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