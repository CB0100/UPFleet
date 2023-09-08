$(document).ready(function () {

    $('#btnid').click(function () {
        var isBargeNameValid = validateBargeName();
        var isSizeValid = validateSize();
        var isRateValid = validateRate();
        var isDescriptionValid = validateDescription();
        var isOwnerValid = validateOwner();

        var selectedValue = $("#Barge_Name").val();
        var validbarge=filldata(selectedValue);

        if (isBargeNameValid && isSizeValid && isRateValid && isDescriptionValid && isOwnerValid && validbarge) {
            Swal.fire(
                'Saved',
                'Data Saved Successfully',
                'success'
            );

        }
        else {
            return false;
        }
    });

    $('#Barge_Name').on('input', validateBargeName);
    $('#description').on('input', validateDescription);
    $('#size').on('input', validateSize);
    $('#rate').on('input', validateRate);


    $('#owner').on('change', function () {
        validateOwner();
    });

    $('#size').keypress(function (e) {
        blockSpecialCharacter(e);
    });

    $('#rate').keypress(function (e) {
        blockSpecialNumber(e);
    });

    $("#rate").val(parseFloat($("#rate").val()).toFixed(2));

    $("#Barge_Name").blur(function () {
        var selectedValue = $("#Barge_Name").val();
        filldata(selectedValue);
        
    });
    function filldata(selectedValue) {
        $.ajax({
            url: '/Maintenance/GetDetails',
            type: 'GET',
            data: { barge: selectedValue },
            success: function (response) {
                if (response.bargeid != 0) {
                    Swal.fire({
                        title: 'Existing Barge',
                        text: 'There is already a Barge with This Name. So Please enter another name.',
                        icon: 'warning',
                        showCancelButton: false,
                        confirmButtonText: 'OK'
                    }).then(function () {
                        // Focus on the Barge_Name textbox
                        $('#Barge_Name').val('');

                    });
                }
            },
            error: function (error) {
                console.error("Error sending data: " + error);
            }
        });
    }
    function validateBargeName() {
        var value = $('#Barge_Name').val();
        var error = $('#erBarge_Name');

        if (value === '') {
            error.text('Please enter a value.');
            $('#Barge_Name').css('border-color', 'red');
            $('#Barge_Name').focus();
            return false;
        } else if (value.length < 3) {
            error.text('Must be at least 3 characters.');
            return false;
        } else {
            error.text('');
            $('#Barge_Name').css('border-color', 'lightgray');
            return true;
        }
    }

    function validateSize() {
        var value = $('#size').val();
        var error = $('#ersize');

        if (value === '') {
            error.text('Please enter a value.');
            $('#size').css('border-color', 'red');
            $('#size').focus();
            return false;
        } else if (value.length < 3) {
            error.text('Must be at least 3 characters.');
            return false;
        } else {
            error.text('');
            $('#size').css('border-color', 'lightgray');
            return true;
        }
    }

    function validateRate() {
        var value = parseFloat($('#rate').val());
        var error = $('#errate');

        if (isNaN(value) || value === 0) {
            error.text('Rate must be a non-zero number.');
            $('#rate').css('border-color', 'red');
            $('#rate').focus();
            return false;
        } else {
            error.text('');
            $('#rate').css('border-color', 'lightgray');
            return true;
        }
    }

    function validateDescription() {
        var value = $('#description').val();
        var error = $('#erdescription');

        if (value === '') {
            error.text('Please enter a value.');
            $('#description').css('border-color', 'red');
            $('#description').focus();
            return false;
        } else if (value.length < 3) {
            error.text('Must be at least 3 characters.');
            return false;
        } else {
            error.text('');
            $('#description').css('border-color', 'lightgray');
            return true;
        }
    }

    function validateOwner() {
        var ownerValue = $('#owner').val();
        var ownerError = $('#erowner');

        if (ownerValue === '0') {
            ownerError.text('Please select an owner.');
            $('#owner').css('border-color', 'red');
            $('#owner').focus();
            return false;
        } else {
            ownerError.text('');
            $('#owner').css('border-color', 'lightgray');
            return true;
        }
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
        var currentValuelength = currentValue.length;


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
            // Check if the cursor position is after the decimal point
            var cursorPosition = e.target.selectionStart;
            if (cursorPosition > decimalIndex) {
                e.preventDefault();
            }
        }
        if (currentValuelength === 0 && key === '.') {
            e.preventDefault();
        }
    }
});