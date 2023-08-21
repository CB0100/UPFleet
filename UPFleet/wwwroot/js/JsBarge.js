﻿$(document).ready(function () {
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

    $('#owner').on('change', function () {
        getValidation();
    });

    $('#size').keypress(function (e) {
        blockSpecialCharacter(e);
    });

    $('#rate').keypress(function (e) {
        blockSpecialNumber(e);
    });
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

    if (!regex.test(key)) {
        e.preventDefault();
    }
}
