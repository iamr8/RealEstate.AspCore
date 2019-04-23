function removeButtonEvent(jsonElement, thisItem) {
    const keyId = $(thisItem).attr("data-id");
    const jsonValue = $.parseJSON($(jsonElement).val());
    const index = jsonValue.findIndex(prop => prop.k === keyId);
    removeJson(jsonElement, index);
    $(thisItem).remove();
}

function toggleResultShown(searchElement, event) {
    const hasFocus = $(searchElement).is(event.target);
    if (!hasFocus && $(searchElement).has(event.target).length === 0) {
        $(searchElement).addClass('d-none');
    }

    if (hasFocus) {
        const parent = $(searchElement).parent();
        const items = $(".result > .items > li", parent).not(".template");

        if (items.length > 0) {
            $(searchElement).removeClass('d-none');
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
    $(idElement).val(currentId).trigger("change");
    $(parent).addClass("d-none");
}

function cleanResult(parent) {
    $("> li", parent).not(".template").remove();
};

function removeJson(jsonElement, index) {
    if ($(jsonElement).val() === "") {
        $(jsonElement).val("[]");
    }

    const jsonString = $(jsonElement).val();
    const json = $.parseJSON(jsonString);

    json.splice(index, 1);
    $(jsonElement).val(JSON.stringify(json)).trigger("change");
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
    $(jsonElement).val(restringJson).trigger("change");
}