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
  $("#pageSTATUS .close").on("click",
    function (e) {
      $("#pageSTATUS").remove();
    });

  $("a.sectiongoto").on("click", function (event) {
    if (this.hash !== "") {
      event.preventDefault();
      const hash = this.hash;
      $("html, body").animate(
        {
          scrollTop: $(hash).offset().top
        },
        800,
        function () {
          window.location.hash = hash;
        }
      );
    }
  });
  $(document).on("click", "[data-confirm]", function (e) {
    if (!confirm($(this).attr("data-confirm"))) {
      e.preventDefault();
      e.stopPropagation();
      return false;
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
    const code = e.keyCode ? e.keyCode : e.which;
    if (code === 13) {
      e.preventDefault();
      return false;
    }
  });

  $('[data-toggle="tooltip"]').tooltip();
});
$("#mainNavBar > .navbar-toggler").click(function (e) {
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

function toggleResultShown(searchElement, event) {
  const hasFocus = $(searchElement).is(event.target);
  if (!hasFocus && $(searchElement).has(event.target).length === 0) {
    $(searchElement).addClass("d-none");
  }

  if (hasFocus) {
    const parent = $(searchElement).parent();
    const items = $(".result > .items > li", parent).not(".template");

    if (items.length > 0) {
      $(searchElement).removeClass("d-none");
    }
  }
}

function resetIdEvent(searchElement, func) {
  const parent = $(searchElement).parent();
  const items = $(".result > .items", parent);

  cleanResult(items);
  func.apply();
}

function foundItemEvent(idElement, thisElement) {
  const parent = $(thisElement).parent();
  const currentId = $(thisElement).attr("data-id");
  $(idElement)
    .val(currentId)
    .trigger("change");
  $(parent).addClass("d-none");
}

function cleanResult(parent) {
  $("> li", parent)
    .not(".template")
    .remove();
}

function removeJson(jsonElement, index) {
  if ($(jsonElement).val() === "") {
    $(jsonElement).val("[]");
  }

  const jsonString = $(jsonElement).val();
  const json = $.parseJSON(jsonString);

  console.log("IndexToDelete:", index);
  json.splice(index, 1);
  $(jsonElement)
    .val(JSON.stringify(json))
    .trigger("change");
}

function addJson(jsonElement, item, uniqueCheck) {
  if ($(jsonElement).val() === "" || $(jsonElement).val() === null) {
    $(jsonElement).val("[]");
  }
  console.log("item to AddToJson:", item);

  const jsonString = $(jsonElement).val();
  console.log("jsonElement:", jsonElement, "jsonString:", jsonString);

  const json = $.parseJSON(jsonString);
  const found = json.findIndex(uniqueCheck) >= 0;
  if (found) {
    return;
  }

  json.push(item);
  const restringJson = JSON.stringify(json);
  $(jsonElement)
    .val(restringJson)
    .trigger("change");
}

function wordifyfa(num, level) {
  "use strict";
  if (num === null) {
    return "";
  }
  if (num === 0) {
    if (level === 0) {
      return "صفر";
    } else {
      return "";
    }
  }
  var result = "",
    yekan = [
      " یک ",
      " دو ",
      " سه ",
      " چهار ",
      " پنج ",
      " شش ",
      " هفت ",
      " هشت ",
      " نه "
    ],
    dahgan = [
      " بیست ",
      " سی ",
      " چهل ",
      " پنجاه ",
      " شصت ",
      " هفتاد ",
      " هشتاد ",
      " نود "
    ],
    sadgan = [
      " یکصد ",
      " دویست ",
      " سیصد ",
      " چهارصد ",
      " پانصد ",
      " ششصد ",
      " هفتصد ",
      " هشتصد ",
      " نهصد "
    ],
    dah = [
      " ده ",
      " یازده ",
      " دوازده ",
      " سیزده ",
      " چهارده ",
      " پانزده ",
      " شانزده ",
      " هفده ",
      " هیجده ",
      " نوزده "
    ];
  if (level > 0) {
    result += " و ";
    level -= 1;
  }

  if (num < 10) {
    result += yekan[num - 1];
  } else if (num < 20) {
    result += dah[num - 10];
  } else if (num < 100) {
    result +=
      dahgan[parseInt(num / 10, 10) - 2] + wordifyfa(num % 10, level + 1);
  } else if (num < 1000) {
    result +=
      sadgan[parseInt(num / 100, 10) - 1] + wordifyfa(num % 100, level + 1);
  } else if (num < 1000000) {
    result +=
      wordifyfa(parseInt(num / 1000, 10), level) +
      " هزار " +
      wordifyfa(num % 1000, level + 1);
  } else if (num < 1000000000) {
    result +=
      wordifyfa(parseInt(num / 1000000, 10), level) +
      " میلیون " +
      wordifyfa(num % 1000000, level + 1);
  } else if (num < 1000000000000) {
    result +=
      wordifyfa(parseInt(num / 1000000000, 10), level) +
      " میلیارد " +
      wordifyfa(num % 1000000000, level + 1);
  } else if (num < 1000000000000000) {
    result +=
      wordifyfa(parseInt(num / 1000000000000, 10), level) +
      " تریلیارد " +
      wordifyfa(num % 1000000000000, level + 1);
  }
  return result;
}