// Write your JavaScript code.
function formatDate(date) {
    var monthNames = [
        "Jan", "Feb", "Mar",
        "Apr", "May", "Jun", "Jul",
        "Aug", "Sep", "Oct",
        "Nov", "Dec"
    ];

    var d1 = parseDate(date);
    var day = d1.getDate();
    var monthIndex = d1.getMonth();
    var year = d1.getFullYear();

    return (day < 10 ? '0' + day : day) + '-' + monthNames[monthIndex] + '-' + year;
}

function parseDate(input) {
    var parts = input.match(/(\d+)/g);
    // new Date(year, month [, date [, hours[, minutes[, seconds[, ms]]]]])
    return new Date(parts[0], parts[1] - 1, parts[2]); // months are 0-based
}