$(document).ready(function () {
    const url = "/Reports/Owner_list/";
    var isfirst = true;
    LoadOwners();
    LoadBarges("All");

    $("#prntbtn").click(function () {
        $(".function-btn").hide();
        $(".filterdiv").hide();
        window.print();
        $(".function-btn").show();
        $(".filterdiv").show();
    });

    $('#ownerDropdown').change(function () {
        const SelectOwner = $('#ownerDropdown').val();
        LoadBarges(SelectOwner);
    });

    function LoadOwners() {
        const dropdownOwner = $('#ownerDropdown');

        $.ajax({
            url,
            method: "GET",
            beforeSend: function () {
                $('#loader-overlay').show();  // Show loader before sending request
            },
            success: function (data) {
                dropdownOwner.empty();
                data.forEach(function (item) {
                    dropdownOwner.append($("<option>").val(item.ownerName).text(item.ownerName));
                });                
                $('#loader-overlay').hide();
            }
        });
    }
    function LoadBarges(SelectOwner) {        
        const url = "/Reports/BargeByOwner/";

        $.ajax({
            url,
            data: { SelectOwner },
            method: "GET",
            beforeSend: function () {
                $('#loader-overlay').show();  // Show loader before sending request
            },
            success: function (data) {
                const tableBody = $("#table-body");
                tableBody.empty(); // Clear existing rows

                data.forEach(function (item) {
                    const row = $("<tr>");
                    row.append($("<td>").text(item.barge_Name));
                    row.append($("<td>").text(item.size));
                    row.append($("<td>").text(item.rate));
                    row.append($("<td>").text(item.owner));
                    row.append($("<td>").text(item.description));
                    // Append more <td> elements for other columns

                    tableBody.append(row);
                });
                if (!isfirst) {
                    $('#loader-overlay').hide();
                }     
                isfirst = false;
            }
        });
    }
});
