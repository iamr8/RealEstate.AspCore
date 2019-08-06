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

function removeURLParameter(url, parameter) {
  //prefer to use l.search if you have a location/link object
  const urlparts = url.split("?");
  if (urlparts.length >= 2) {
    const prefix = encodeURIComponent(parameter) + "=";
    const pars = urlparts[1].split(/[&;]/g);

    //reverse iteration as may be destructive
    for (let i = pars.length; i-- > 0;) {
      //idiom for string.startsWith
      if (pars[i].lastIndexOf(prefix, 0) !== -1) {
        pars.splice(i, 1);
      }
    }

    url = urlparts[0] + "?" + pars.join("&");
    return url;
  } else {
    return url;
  }
}

$(document).ready(function () {
  $("#pageSTATUS .close").on("click",
    function (e) {
      if (location.href.includes("?")) {
        const splitUrl = location.href.split("?");
        const [url, queryString] = splitUrl;
        const params = queryString.split("&");
        var newParams = [];
        var indicator = 0;
        $.each(params,
          (index, param) => {
            const [key, value] = param.split("=");
            if (key !== "status") {
              newParams[indicator] = `${key}=${value}`;
              indicator++;
            }
          });
        const newQueryString = newParams.join("&");
        console.log("Array: ", newParams, "QueryString: ", newQueryString);
        history.pushState({}, document.title, `?${newQueryString}`);
        $("#pageSTATUS").remove();
      }
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

function showLoadingModal() {
  const modal = document.createElement("div");
  $(modal).addClass("modal fade");
  $(modal).attr("id", "loadingModal");
  $(modal).attr("data-backdrop", "static");
  $(modal).attr("data-keyboard", "false");
  $(modal).attr("tabindex", "-1");
  const modalDialog = document.createElement("div");
  $(modalDialog).addClass("modal-dialog").addClass("modal-sm");

  const modalContent = document.createElement("div");
  $(modalContent).addClass("modal-content justify-content-center flex-row");

  const loadingSpinner = document.createElement("div");
  $(loadingSpinner).addClass("spinner-grow");

  const loadingTitle = document.createElement("span");
  $(loadingTitle).addClass("title");
  $(loadingTitle).html("لطفا کمی صبر کنید");

  $(modalContent).append(loadingSpinner).append(loadingTitle);
  $(modalDialog).append(modalContent);
  $(modal).append(modalDialog);

  return $(modal);
}

$(document).ready(function () {
  $(".page-link").click(function (e) {
    e.preventDefault();
    const pageNo = parseInt($(this).html());

    const splitUrl = location.href.split("?");
    var [url, queryString] = splitUrl;
    if (url.endsWith("#"))
      url = url.split("#")[0];

    var params = [];
    if (queryString !== undefined && queryString !== "" && queryString !== null)
      params = queryString.split("&");

    console.log("caughtParams", params);
    var newParams = [];
    var indicator = 0;

    var model = new Object();
    var currentPageNo = 0;
    var shouldBeAdded = true;
    $.each(params,
      (index, param) => {
        var [key, value] = param.split("=");
        if (value.endsWith("#"))
          value = value.split("#")[0];

        if (key === "pageNo") {
          currentPageNo = parseInt(value);
          value = pageNo;
          shouldBeAdded = false;
        }

        model[key] = value;
        newParams[indicator] = `${key}=${value}`;
        indicator++;
      });

    if (shouldBeAdded) {
      model["pageNo"] = pageNo;
      newParams[indicator] = `pageNo=${pageNo}`;
    }

    console.log("currentPageNo", currentPageNo, "nextPageNo", pageNo);
    console.log("newParams", newParams);
    console.log("payLoad object:", model);
    const newQueryString = newParams.join("&");

    const newUrl = `${url}?${newQueryString}`;
    console.log(newUrl);

    if (typeof history.pushState === "undefined") {
      // unsupported browsers
      window.location.href = newUrl;
    } else {
      // supported browsers
      console.log("Start to get pageItems");

      const modalLoading = showLoadingModal();
      $("body").append(modalLoading);
      $("#loadingModal").modal("show");

      $(".grid").fadeOut("slow",
        function () {
          $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            url: `${url}?handler=Page`,
            data: { json: JSON.stringify(model) }
          }).done(function (response, status, xhr) {
            console.log("requestStatus", status);
            if (xhr.status === 200) {
              // hide loading
              const pageItems = $(".pagination").children();

              console.log("start to select correct pageLink");
              $.each(pageItems,
                (key, value) => {
                  const page = parseInt($(".page-link", value).text());
                  if (page === pageNo) {
                    $(value).addClass("active");
                  } else {
                    if ($(value).hasClass("active")) {
                      $(value).removeClass("active");
                    }
                  }
                });

              const top = $("#paginatedList").position().top;
              $("html,body").animate({ scrollTop: top }, "slow");
              console.log("Finished pageItems");
              window.history.pushState({}, "", newUrl);
              $(".grid").html(response);
              //          $(".grid").isotope("reloadItems").isotope();
            }
            $("#loadingModal").modal("hide");
            $(".grid").fadeIn("slow");
          });
        });
    }
  });

  const rowsAttr = $(".grid").attr("data-rows");
  console.log("Row Attribute:", rowsAttr);
  if (rowsAttr !== undefined) {
    const rows = parseInt(rowsAttr);
    console.log("Rows Count:", rows);
    if (rows > 0) {
      $(".page-link", ".page-item.active").trigger("click");
    }
  }
});