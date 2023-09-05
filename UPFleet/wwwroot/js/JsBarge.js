
$(document).ready(function () {
    $('#loader-overlay').hide();
    $('#btnid').click(function () {
        if (!getValidation()) {
            return false;
        }
        else {
            Swal.fire(
                'Saved',
                'Data Saved Successfully',
                'success'
            );
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

    function handleDuplicateBarges(response, formData) {
        if (response.totalnewbarges > 0) {
            Swal.fire({
                title: 'Are you sure?',
                text: "You won't be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    proceedWithImport(formData);
                }
                else {

                }
            })
            if (confirm("Total " + response.totalduplicatebarge + " Duplicate Barges Found. \n There are" + response.totalnewbarges + " New Barges in this file.\n\nDo you want to skip duplicate barges or cancel importing?")) {
                // Skip the duplicate barge and continue
                
            } else {
                Swal.fire(
                    'Cancelled',
                    'Import process canceled.',
                    'error'
                );
            }
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
                    'Error importing data: \n' + error.responseText,
                    'error'
                ); 
            }
        });
    }


    function getValidation() {
        var isValid = true;

        $('#erBarge_Name').text('');
        $('#ersize').text('');
        $('#errate').text('');
        $('#erdescription').text('');
        $('#erowner').text('');

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
                isValid = false;
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