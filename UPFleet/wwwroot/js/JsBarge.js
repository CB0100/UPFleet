$(document).ready(function () {
    $('#btnid').click(function () {
        if (!getValidation()) {
            return false;
        }
        else {
			alert("Data Saved Successfully");
        }
    });

    $('#formid input').on('change', function () {
        getValidation();
    });
    
    $("#Barge_Name").autocomplete({
        source: function (request, response) {            
            $.ajax({
                url: '/Maintenance/AutocompleteBarge', 
                type: 'GET',                    
                data: { term: request.term },    
                success: function (data) {
                    response(data);
                }
            });
        },
        select: function (event, ui) {
            // Handle the selected autocomplete suggestion
            var selectedValue = ui.item.value;

            // Send the selected value to the MVC action using AJAX
            $.ajax({
                url: '/Maintenance/GetDetails',
                type: 'GET',
                data: { barge: selectedValue },
                success: function (response) {
                    if (response.bargeid != 0) {
                        window.location.href = '/Maintenance/BargeUpdate/' + response.bargeid;
                    }
                },
                error: function (error) {
                    console.error("Error sending data: " + error);
                }
            });      
        }
    });
    $('#owner').on('change', function () {
        getValidation();
    });

    $('#size').keypress(function (e) {
        blockSpecialCharacter(e);
    });

    $('#rate').keypress(function (e) {
        blockSpecialNumber(e);
    });

    $("#rate").val(parseFloat($("#rate").val()).toFixed(2));
});

function getValidation() {
    var isValid = true;

    var inputs = [
        { selector: '#Barge_Name', errorSelector: '#erBarge_Name', minLength: 3 },
        { selector: '#size', errorSelector: '#ersize', minLength: 3 },
        { selector: '#rate', errorSelector: '#errate', minLength: 2 },
        { selector: '#description', errorSelector: '#erdescription', minLength: 3 }
    ];

    inputs.forEach(function (input) {
        var value = $(input.selector).val();
        var error = $(input.errorSelector);

        if (value === '') {
            error.text('Please enter a value.');
            $(input.selector).css('border-color', 'red');
            $(input.selector).focus();
            isValid = false;
        } else if (value.length < input.minLength) {
            error.text('Must be at least ' + input.minLength + ' characters.');
        } else {
            error.text('');
            $(input.selector).css('border-color', 'lightgray');
        }
    });

    var ownerValue = $('#owner').val();
    var ownerError = $('#erowner');

    if (ownerValue === '0') {
        ownerError.text('Please select an owner.');
        $('#owner').css('border-color', 'red');
        $('#owner').focus();
    } else {
        ownerError.text('');
        $('#owner').css('border-color', 'lightgray');
    }

    return isValid;
}

function blockSpecialCharacter(e) {
    var key = e.key;
    var regex = /[0-9a-zA-Z]/;

    if (!regex.test(key)) {
        e.preventDefault();
    }
}

function blockSpecialNumber(e) {
    var key = e.key;
    var regex = /[0-9.]/;

    var currentValue = e.target.value;
    var decimalIndex = currentValue.indexOf('.');

    // Allow only digits and a single decimal point
    if (!regex.test(key)) {
        e.preventDefault();
    }

    // Allow only one decimal point
    if (key === '.' && decimalIndex !== -1) {
        e.preventDefault();
    }

    // Allow only two digits after the decimal point
    if (decimalIndex !== -1 && currentValue.substring(decimalIndex + 1).length >= 2) {
        e.preventDefault();
    }
}