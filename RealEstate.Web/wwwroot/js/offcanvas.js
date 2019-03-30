var isToggledOn = true;

$(document).ready(function () {
    syncState();
});

$(window).resize(function () {
    syncState();
});

function syncState() {
    const width = $(window).width();

    if (width >= 769) {
        isToggledOn = true;
    } else {
        isToggledOn = !isToggledOn;
    }

    console.log("Toggle state:", isToggledOn, "On screen width:", width);

    if (isToggledOn) {
        $(".backdrop").addClass("in");
        $("#navbarNavDropdown").addClass("in");
        $(".navbar").addClass("in");
        $("main.container-fluid").addClass("in");
    } else {
        $(".backdrop").removeClass("in");
        $("#navbarNavDropdown").removeClass("in");
        $(".navbar").removeClass("in");
        $("main.container-fluid").removeClass("in");
    }
}

$(".navbar-toggler").click(function (e) {
    syncState();
});

$(".backdrop").click(function (e) {
    if ($(this).hasClass("in")) {
        syncState();
    }
});