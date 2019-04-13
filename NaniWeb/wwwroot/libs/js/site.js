function validateGenericInput3Chars(genericInput) {
    return genericInput.length >= 3;
}

function validatePassword(password) {
    return password.length >= 8;
}

function validateEmail(email) {
    const regex = new RegExp("[a-z0-9!#$%&'*+/=?^_`{|}~.-]+@[a-z0-9-]+(\\.[a-z0-9-]+)*");
    
    return regex.test(email);
}

async function subscribe() {
    await messaging.requestPermission();
    const token = await messaging.getToken();

    const xhr = new XMLHttpRequest();
    xhr.open("GET", subscribeUrl + "/" + fcmTopic + "/" + token, true);
    xhr.send();
    window.localStorage.setItem(fcmTopic,"Subscribed");
}

async function unsubscribe() {
    const token = await messaging.getToken();

    const xhr = new XMLHttpRequest();
    xhr.open("GET", unsubscribeUrl + "/" + fcmTopic + "/" + token, true);
    xhr.send();
    window.localStorage.setItem(fcmTopic,"Not subscribed");
}

function previous() {
    if (index > 0) {
        index--;
        blur();
        page.attr("src", pages[index]);
        $('#page_selector').val(index);
        scrollToTop();
        previousPreload();
    }
}

function next() {
    if (index < pages.length - 1) {
        index++;
        blur();
        page.attr("src", pages[index]);
        $('#page_selector').val(index);
        scrollToTop();
        nextPreload();

        if (index === pages.length - 1) {
            $("#bottom").removeClass("d-none");
        }
    } else {
        window.location = nextAddress;
    }
}

function blur() {
    const img = $("#manga_page");
    const spinner = $("#load_spinner");

    img.addClass("blur");
    spinner.removeClass("d-none");
    spinner.addClass("d-flex");
}

function removeBlur() {
    const img = $("#manga_page");
    const spinner = $("#load_spinner");

    img.removeClass("blur");
    spinner.removeClass("d-flex");
    spinner.addClass("d-none");
}

function nextPreload() {
    for (var i = 1; i <= 2; i++) {
        if (index + i < pages.length) {
            const image = new Image().src = pages[index + i];
        }
    }
}

function previousPreload() {
    for (let i = 1; i <= 2; i++) {
        if (index - i >= 0) {
            const image = new Image().src = pages[index - i];
        }
    }
}

function scrollToTop() {
    $(window).scrollTop($('#pages').offset().top);
}