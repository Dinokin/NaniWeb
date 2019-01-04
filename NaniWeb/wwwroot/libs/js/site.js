function validateGenericInput3Chars(genericInput) {
    return genericInput.length >= 3;
}

function validatePassword(password) {
    return password.length >= 8;
}

function validateEmail(email) {
    const regex = new RegExp("^[^\\s]+@[^\\s]+$");

    console.log(email);

    return regex.test(email) && email.length >= 3;
}