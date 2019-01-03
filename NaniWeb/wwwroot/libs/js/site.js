function validateGenericInput3Chars(genericInput) {
    return genericInput.length >= 3;
}

function validateUsername(username) {
    const regex = new RegExp("^[^\\s]+$");

    return regex.test(username) && username.length >= 3;
}

function validatePassword(password) {
    const regex = new RegExp("^[^\\s]+$");

    return regex.test(password) && password.length >= 8;
}

function validateEmail(email) {
    const regex = new RegExp("^[^\\s]+@[^\\s]+$");

    console.log(email);

    return regex.test(email) && email.length >= 3;
}