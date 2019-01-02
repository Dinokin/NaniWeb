function subscribe() {
    messaging.requestPermission()
        .then(function() {
            return messaging.getToken();
        })
        .then(function(token) {
            const xhr = new XMLHttpRequest();
            xhr.open("POST", unsubscribeUrl + "/" + fcmTopic + "/" + token, true);
            xhr.send();
            window.localStorage.setItem(fcmTopic,"Subscribed");
        })
        .catch(function(err) {
            console.log(err);
        });
}

function unsubscribe() {
    const xhr = new XMLHttpRequest();
    xhr.open("POST", unsubscribeUrl + "/" + fcmTopic + "/" + messaging.getToken(), true);
    xhr.send();
    window.localStorage.setItem(fcmTopic,"Not subscribed");
}

$("#fcm_toggle").toggle(function() {
    if (this.is(":checked"))
        subscribe();
    else
        unsubscribe();
});