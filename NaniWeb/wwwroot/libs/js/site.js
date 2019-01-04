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