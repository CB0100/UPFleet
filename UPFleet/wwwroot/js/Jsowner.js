$(document).ready(function () {
    var owner_name, company, account;

    $('#formid').on('change', function () {
        GetValidation();
    });
    $("#owner_name").blur(function () {
        var selectedValue = $("#owner_name").val();
        filldata(selectedValue);
    });

    $("#owner_name").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Maintenance/AutocompleteOwner',
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
            filldata(selectedValue);
            
        }
    });
    $('#btnid').click(function () {
        if (!GetValidation()) {
            return false;
        }
    });

    function GetValidation() {
        owner_name = $('#owner_name').val();
        company = $('#company').val();
        account = $('#account').val();

        return validateField(owner_name, $('#erowner_name'), 'owner_name', 3) &&
            validateField(company, $('#ercompany'), 'company', 3) &&
            validateField(account, $('#eraccount'), 'account', 1, true);
    }

    function filldata(selectedValue) {
        // Send the selected value to the MVC action using AJAX
        $.ajax({
            url: '/Maintenance/GetDetails',
            type: 'GET',
            data: { owner: selectedValue },
            success: function (response) {
                if (response.ownerid != 0) {
                    window.location.href = '/Maintenance/OwnerUpdate/' + response.ownerid;
                }
            },
            error: function (error) {
                console.error("Error sending data: " + error);
            }
        });
    }
    function validateField(value, errorElement, fieldName, minLength, isNumberField = false) {
        if (value === "") {
            errorElement.text('please enter ' + fieldName);
            $('#' + fieldName).css('border-color', 'red').focus();
            return false;
        } else if (value.length < minLength) {
            errorElement.text(fieldName + ' must be greater than or equal to ' + minLength + ' characters');
            return false;
        } else {
            errorElement.text('');
            $('#' + fieldName).css('border-color', 'lightgray');
            return true;
        }
    }

    $('#owner_name, #company').keypress(function (e) {
        var result = BlockSpecialCharacter(e);
        if (!result) {
            e.preventDefault();
        } else {
            hideError($(this));
        }
    });

    $('#account').keypress(function (e) {
        var result = BlockSpecialNumber(e);
        if (!result) {
            e.preventDefault();
        } else {
            hideError($(this));
        }
    });

    function showError(inputElement, errorMessage) {
        var errorElement = $('#er' + inputElement.attr('id'));
        errorElement.text(errorMessage);
    }

    function hideError(inputElement) {
        $('#er' + inputElement.attr('id')).text('');
    }

    function BlockSpecialCharacter(e) {
        var keyCharCode = e.key.charCodeAt(0);
        return (keyCharCode >= 48 && keyCharCode <= 57) || // 0-9
            (keyCharCode >= 65 && keyCharCode <= 90) || // A-Z
            (keyCharCode >= 97 && keyCharCode <= 122); // a-z
    }

    function BlockSpecialNumber(e) {
        var keyCharCode = e.key.charCodeAt(0);
        return keyCharCode >= 48 && keyCharCode <= 57; // 0-9
    }
});
