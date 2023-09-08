
$(document).ready(function () {
    $('#loader-overlay').hide();
    $('#btnid').click(function () {
        var isBargeNameValid = validateBargeName();
        var isSizeValid = validateSize();
        var isRateValid = validateRate();
        var isDescriptionValid = validateDescription();
        var isOwnerValid = validateOwner();

        if (isBargeNameValid && isSizeValid && isRateValid && isDescriptionValid && isOwnerValid) {
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

    $("#Barge_Name").blur(function () {
        var selectedValue = $("#Barge_Name").val();
        filldata(selectedValue);
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
            filldata(selectedValue);            
        }
    });   
    $("#rate").val(parseFloat($("#rate").val()).toFixed(2));



    $('#exportBtn').click(function (event) {
        event.preventDefault(); // Prevent the default form submission behavior

        $.ajax({
            url: '/Maintenance/ExportBargesToExcel', // Make sure the URL is correct
            type: 'GET',
            xhrFields: {
                responseType: 'blob' // Set the response type to blob
            },
            success: function (blob) {
                // Save the blob as a file using FileSaver.js
                saveAs(blob, 'Barges.xlsx');
            },

            error: function (error) {
                console.error("Error exporting data: ", error);
            }
        });
    });


    $('#importBtn').click(function (event) {
        event.preventDefault();
        // Prevent the default form submission behavior
        // Create a hidden file input element
        var fileInput = document.createElement('input');
        fileInput.type = 'file';
        fileInput.accept = '.xlsx, .xls';
        fileInput.style.display = 'none';

        // Attach change event handler to the hidden input
        fileInput.addEventListener('change', function () {
            var file = fileInput.files[0];

            if (file) {
                var formData = new FormData();
                formData.append('excelFile', file);

                // Start the import process
                importBarges(formData);
            }
        });

        // Trigger click on the hidden file input
        fileInput.click();
    });

    function importBarges(formData) {
        $.ajax({
            url: '/Maintenance/CheckDuplicateBarge',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function () {
                $('#loader-overlay').show();  // Show loader before sending request
            },
            success: function (response) {
                $('#loader-overlay').hide();
                if (response.isDuplicate) {
                    handleDuplicateBarges(response, formData);
                } else {
                    proceedWithImport(formData);
                }
            },
            error: function (error) {
                $('#loader-overlay').hide();
                alert("Error checking duplicate barge: " + error.responseText);
            }
        });
    }
    function filldata(selectedValue) {

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

    function handleDuplicateBarges(response, formData) {
        if (response.totalnewbarges > 0) {
            Swal.fire({
                title: response.totalnewbarges + ' New Barges in this file.\n' + response.totalduplicatebarge + ' Duplicate Barges Found.',
                text: 'Do you want to skip duplicate barges and import or cancel importing?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Skip Duplicates and Import',
                cancelButtonText: 'Cancel Importing'
            }).then((result) => {
                if (result.isConfirmed) {
                    proceedWithImport(formData);
                }
                else {
                    Swal.fire(
                        'Cancelled',
                        'Import process canceled.',
                        'error'
                    );
                }
            })
        }
        else {
            Swal.fire(
                'No Additional Barges',
                'All Barges are already Stored. No New Barge Found in this Excel File.',
                'warning'
            );           
        }
    }
        
    function proceedWithImport(formData) {
        $.ajax({
            url: '/Maintenance/ImportBargesFromExcel', // Server URL for import
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function () {
                $('#loader-overlay').show(); // Show loader before sending request
            },
            success: function (response) {
                $('#loader-overlay').hide();
                // Handle successful import response
                alert("Import successful:", response);
            },
            error: function (error) {
                $('#loader-overlay').hide();
                // Show an alert for other errors
                Swal.fire(
                    'Error importing data:',
                    '\n' + error.responseText,
                    'error'
                ); 
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