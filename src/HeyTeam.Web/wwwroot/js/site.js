// Write your JavaScript code.
function formatDate(date, format) {
    if (format == 'ddMMMyyyy')
        return ddMMMyyyy(date);
    else if (format == 'yyyyMMdd')
        return yyyyMMdd(date);
    else
        return date;
}

function yyyyMMdd(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
}

function ddMMMyyyy(date) {
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

Date.prototype.getWeek = function () {
    var onejan = new Date(this.getFullYear(), 0, 1);
    var today = new Date(this.getFullYear(), this.getMonth(), this.getDate());
    var dayOfYear = ((today - onejan + 1) / 86400000);
    return Math.ceil(dayOfYear / 7)
};

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement).replace(/^ +| +$/gm, ""); //replace leading spaces while preserving line breaks
};

function ajax(url, method, data, successCallback, failureCallback) {
    $.ajax({
        method: method,
        url: url,
        data: data
    }).done(function (data) {        
        successCallback(data);
    }).fail(function (xhr, textStatus, error) {
        failureCallback();
    })
}