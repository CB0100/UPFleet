$(document).ready(function () {

    loadBarges();
    // Call the loadBarges function when the ownerDropdown changes
    $('#ownerDropdown').change(function () {
        loadBarges();
    });

    // Call the Okbutton function when the OK button is clicked
    $('#okButton').click(function () {
        Okbutton();
    });

    function loadBarges() {
        var selectedOwner = $('#ownerDropdown').val();

        // You can replace this URL with the actual URL for fetching barge data based on owner
        var url = "/Home/GetBargesByOwner?owner=" + encodeURIComponent(selectedOwner);
        // Perform an AJAX request to get barge data
        $.ajax({
            url: url,
            method: "GET",
            success: function (data) {
                var bargeDropdown = $('#bargeDropdown');
                bargeDropdown.empty(); // Clear existing options

                if (data.length === 0) {
                    bargeDropdown.append($('<option></option>').text("No barges available"));
                } else {
                    // Populate barge dropdown with new options
                    $.each(data, function (index, barge) {
                        bargeDropdown.append($('<option></option>').val(barge.barge_Name).text(barge.barge_Name));
                    });
                }
            },
            error: function () {
                console.log("Error loading barges");
            }
        });
    }

    function Okbutton() {
        var selectedOption = $('#bargeDropdown').val();
        if (selectedOption) {
            var value = $('select[id=bargeDropdown] option:selected').val();
            window.location.href = "/Home/IndexPage?BargeName=" + value;
        } else {
            alert("Please select a barge.");
        }
    }
});