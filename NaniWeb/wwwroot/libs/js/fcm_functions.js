function subscribe() {
    messaging.requestPermission()
        .then(function() {
            return messaging.getToken();
        })
        .then(function(token) {
            const xhr = new XMLHttpRequest();
            xhr.open("GET", subscribeUrl + "/" + fcmTopic + "/" + token, true);
            xhr.send();
            window.localStorage.setItem(fcmTopic,"Subscribed");
        })
        .catch(function(err) {
            console.log(err);
        });
}

async function unsubscribe() {
    const token = await messaging.getToken();
    
    const xhr = new XMLHttpRequest();
    xhr.open("GET", unsubscribeUrl + "/" + fcmTopic + "/" + token, true);
    xhr.send();
    window.localStorage.setItem(fcmTopic,"Not subscribed");
}

$("#fcm_toggle").change(async function() {
    if (this.checked)
        subscribe();
    else
        await unsubscribe();
});