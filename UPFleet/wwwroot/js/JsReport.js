$(document).ready(function () {
    const printButton = $("#prntbtn");
    const paginationContainer = $(".navcontainer");

    printButton.css("display", "initial");
    paginationContainer.css("display", "initial"); // Show the pagination container

    printButton.on("click", function () {
        printButton.css("display", "none");
        paginationContainer.css("display", "none"); // Hide the pagination container
        window.print();
        printButton.css("display", "initial");
        paginationContainer.css("display", "initial"); // Show the pagination container again after printing
    });
});

$(document).ready(function () {
    $("#prntbtn").click(function () {
        $("#backbtn").hide(); // Hide the back button before printing
        $("#prntbtn").hide(); // Hide the print button before printing
        window.print();
        $("#backbtn").show(); // Show the back button after printing
        $("#prntbtn").show(); // Show the print button after printing
    });
});