function openDropdown(id) {
    var element = document.getElementById(id);
    if (element) {
        element.dispatchEvent(new Event("click"));
    }
}

function attachFocusHandler(id, componentClass) {
    var element = document.getElementById(id);
    if (element) {
        element.addEventListener("focus", (event) => {
            var keyEvent = new KeyboardEvent("keydown", {
                "altKey": true,
                "code": "ArrowDown",
                "key": "ArrowDown",
                "keyCode": 40
            });
            if ((componentClass == ".k-multiselect" && (event.relatedTarget && event.relatedTarget != element.parentNode.parentNode))
                || (componentClass != undefined && componentClass != ".k-multiselect")) {
                // AutoComplete, ComboBox, DatePicker, TimePicker - tab, click, FocusAsync
                // MultiSelect - tab, FocusAsync
                // element is an input, so we go up the DOM tree to find the component root
                element.closest(componentClass).dispatchEvent(keyEvent);
            } else {
                // DropDownList - tab, FocusAsync
                // element is a span and this is the component root
                element.dispatchEvent(keyEvent);
            }
        });
    }
}