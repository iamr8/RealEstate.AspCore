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

function removeJson(jsonElement, index) {
    if ($(jsonElement).val() === "") {
        $(jsonElement).val("[]");
    }

    const jsonString = $(jsonElement).val();
    const json = $.parseJSON(jsonString);

    json.splice(index, 1);
    $(jsonElement).val(JSON.stringify(json)).trigger("change");
}

function addJson(jsonElement, item) {
    if ($(jsonElement).val() === "") {
        $(jsonElement).val("[]");
    }

    const jsonString = $(jsonElement).val();
    const json = $.parseJSON(jsonString);

    const found = json.findIndex(obj => obj.k === item.k) >= 0;
    if (found)
        return;

    json.push(item);
    $(jsonElement).val(JSON.stringify(json)).trigger("change");

}