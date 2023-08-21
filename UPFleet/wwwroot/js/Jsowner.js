$(document).ready(function () {
    var $ownerName = $('#owner_name');
    var $company = $('#company');
    var $account = $('#account');

    var $errorOwnerName = $('#erowner_name');
    var $errorCompany = $('#ercompany');
    var $errorAccount = $('#eraccount');

    $('#formid').on('input', GetValidation);

    $('#btnid').click(function () {
        if (!GetValidation()) {
            return false;
        }
    });

    $ownerName.on('input', function () {
        ValidateInput($ownerName, $errorOwnerName, BlockSpecialCharacter);
    });

    $company.on('input', function () {
        ValidateInput($company, $errorCompany, BlockSpecialCharacter);
    });

    $account.on('input', function () {
        ValidateInput($account, $errorAccount, BlockSpecialNumber);
    });
});

function ValidateInput($input, $error, validationFunction) {
    var value = $input.val();

    if (value === "") {
        $error.text('Please enter a value');
        $input.addClass('invalid');
        return false;
    } else if (value.length < 3) {
        $error.text('Must be at least 3 characters');
        $input.addClass('invalid');
        return false;
    } else if (validationFunction && !validationFunction(value)) {
        $error.text('Special characters not allowed');
        $input.val('');
        $input.addClass('invalid');
        return false;
    } else {
        $error.text('');
        $input.removeClass('invalid');
        return true;
    }
}

function BlockSpecialCharacter(value) {
    var regex = /^[a-zA-Z0-9]*$/;
    return regex.test(value);
}

function BlockSpecialNumber(value) {
    var regex = /^[0-9]*$/;
    return regex.test(value);
}
