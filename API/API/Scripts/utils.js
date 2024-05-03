$(document).ready(function () {

    // Get states.
    $("#countrySelect").on("change", function () {
        var countryName = $("#countrySelect").val();
        console.log("Country:", countryName);
        $("#citiesDropdown").html('');

        if (countryName) {
            $.get('/Home/States', { countryName: countryName }, function (response) {
                $("#stateDropdown").html(response);
            });
        } else {
            $("#stateDropdown").html('');
        }
    });
});

function getStates() {
    var countryName = $("#countrySelect").val();
    console.log("Country:", countryName);
    $("#citiesDropdown").html('');

    if (countryName) {
        $.get('/Home/States', { countryName: countryName }, function (response) {
            $("#stateDropdown").html(response);
        });
    } else {
        $("#stateDropdown").html('');
    }
}

function hookOnChange() {
    // Get cities.
    $("#stateSelect").on("change", function () {
        var stateName = $("#stateSelect").val();
        console.log("State:", stateName);

        if (stateName) {
            $.get("/Home/Cities", { stateName: stateName }, function (response) {
                $("#citiesDropdown").html(response);
            });
        } else {
            $("#citiesDropdown").html('');
        }
    });
}
