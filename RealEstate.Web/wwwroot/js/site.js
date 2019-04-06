function delayOnRun(func, delay) {
    var timer = null;
    return function () {
        var context = this,
            args = arguments;
        clearTimeout(timer);
        timer = window.setTimeout(function () {
            func.apply(context, args);
        }, delay || 1000);
    };
}

$(document).ready(function () {
    $("a.sectiongoto").on("click",
        function (event) {
            if (this.hash !== "") {
                event.preventDefault();
                var hash = this.hash;
                $("html, body").animate({
                    scrollTop: $(hash).offset().top
                },
                    800,
                    function () {
                        window.location.hash = hash;
                    });
            }
        });
    $(document).on("click", "a[data-confirm]", function (e) {
        if (!confirm($(this).attr("data-confirm"))) {
            e.preventDefault();
        }
    });
    $(".lazy").lazy({
        effect: "fadeIn",
        visibleOnly: true,
        onError: function (element) {
            console.log(`error loading ${element.data("src")}`);
        }
    });
    $(".no-enter").keypress(function (e) {
        const code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            e.preventDefault();
            return false;
        }
    });
});