function bindEvents(buttonSelector) {
    $(buttonSelector).on('click', function (e) {
        var eventId = $(this).closest('tr').data('eventid');
        var squadId = $(this).closest('tr').data('squadid');
        var playerId = $(this).closest('tr').data('playerid');
        var attendanceid = $(this).data('attendanceid');
        var currentstate = $(this).data('state');
        var button = $(this);

        $.ajax({
            method: "POST",
            contentType: 'application/json',
            url: "/api/events/attendance",
            data: JSON.stringify({ "eventid": eventId, "squadid": squadId, "playerid": playerId, "attendance": (currentstate == 1 ? null : attendanceid) })
        })
            .done(function (data) {
                var buttons = button.closest('tr').find("button");
                clearAttributes(buttons);
                if (currentstate == 0) {
                    var style = (attendanceid == 1 ? "btn-success" : (attendanceid == 2 ? "btn-danger" : (attendanceid == 3 ? "btn-warning" : "btn-info")));
                    $(button).addClass("btn " + style);
                }
                $(button).data("state", (currentstate == 1 ? 0 : 1));
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.log(textStatus);
            });
    });

    function clearAttributes(buttons) {
        for (var i = 0; i < buttons.length; i++) {
            $(buttons[i]).removeClass();
            $(buttons[i]).addClass("btn btn-default");
            $(buttons[i]).data("state", 0);
        }
    }
}