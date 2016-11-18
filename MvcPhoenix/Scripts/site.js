// Check and confirm that user has WCPP installed locally
var wcppPingDelay_ms = 1000;

function wcppDetectOnSuccess() {
    //Check WCPP
    var wcppVer = arguments[0];
    if (wcppVer.substring(0, 1) != "2")
        wcppDetectOnFailure();
}

function wcppDetectOnFailure() {
    //WCPP is not installed. Ask user to install it
    $('#msgInstallWCPP').show();
}

// Calx on change helper used in Invoice RT calculation
$('#show_formula').click(function (e) {
    e.preventDefault();

    if ($(this).attr('data-shown') == '0') {
        $('[data-formula],[data-cell]').each(function () {
            $(this).after('<span class="formula">' + $(this).attr('data-cell') + ($(this).attr('data-formula') ? ' = ' + $(this).attr('data-formula') : '') + '&nbsp;</span>');
        });

        $(this).attr('data-shown', 1).html('Hide Formula');
    } else {
        $('span.formula').remove();
        $(this).attr('data-shown', 0).html('Show Formula');
    }
});

// Custom jquery general helpers
$(function () {
    $("form input").tooltipValidation({
        placement: "top"
    });
    $("form select").tooltipValidation({
        placement: "top"
    });
    $("form textarea").tooltipValidation({
        placement: "top"
    });
});

$(function () {
    $('.date-picker').datepicker({
        dateFormat: 'dd-M-yy'
    });
});

$(function () {
    $('.datetime-picker').datepicker({
        dateFormat: 'dd-MM-yy',
        onSelect: function (datetext) {
            var d = new Date(); // for now
            datetext = datetext + " " + ('0' + d.getHours()).slice(-2) + ": " + ('0' + d.getMinutes()).slice(-2) + ": " + ('0' + d.getSeconds()).slice(-2);
            $(this).val(datetext);
        }
    });
});

// Order/Index - Modal Search
$("#advancedsearch").on('hidden.bs.modal', function () {
    $(this).data('bs.modal', null);
});

// General Helpers
function goBack() {
    window.history.back();
}