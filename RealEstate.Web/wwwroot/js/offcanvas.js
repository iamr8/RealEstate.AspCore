$(".navbar-toggler").click(function (e) {
    $(".backdrop").toggleClass("in");
    $("#navbarNavDropdown").toggleClass("in");
});

$(".backdrop").click(function (e) {
    if ($(this).hasClass("in")) {
        $(".backdrop").removeClass("in");
        $("#navbarNavDropdown").removeClass("in");
        console.log("backdrop clicked");
    }
});

$(".sidenav.in").on("swiperight", function (e) {
    console.log("sidenav clicked");
    $(".backdrop").trigger("click");
});