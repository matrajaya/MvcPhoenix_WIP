// Custom jquery general helpers
$(function () {
    $("form input").tooltipValidation({
        placement: "top"
    });
    $("form select").tooltipValidation({
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

// Order Import JS helper for file upload
function handleFileSelect(evt) {
    var Files = evt.target.files; // FileList object

    // files is a FileList of File objects. List some properties.
    var output = [];
    for (var i = 0, f; f = Files[i]; i++) {
        output.push('<li><strong>', escape(f.name), '</strong> (', f.type || 'n/a', ') - ',
                    f.size, ' bytes, last modified: ',
                    f.lastModifiedDate ? f.lastModifiedDate.toLocaleDateString() : 'n/a',
                    '</li>');
    }
    document.getElementById('list').innerHTML = '<ul>' + output.join('') + '</ul>';
}

document.getElementById('Files').addEventListener('change', handleFileSelect, false);