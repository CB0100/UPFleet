$(document).ready(function () {
    $('#barge_rate').on('input', function () {
        var value = $(this).val();
        value = parseFloat(value).toFixed(2);
        $(this).val(value);
    });
});