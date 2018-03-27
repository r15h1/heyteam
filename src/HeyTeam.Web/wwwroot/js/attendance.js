var attendance = { unSpecified: null, present: 1, noShow: 2, late: 3, leftEarly: 4 };
var columns = { squad: 1, player: 2, present: 3, late: 4, leftEarly: 5, noShow: 6, timeLogged: 7 };
var timeLogForm = "<form class='time-log'></form>";
var timeLogInput = "<input class='form-control time-input input-sm' type='number' min='0' max='400' maxlength='3' value='0' readonly placeholder='0'/>";
var timeLogOkButton = "<button type='submit' class='btn btn-default btn-sm saveTimeLog' title='save'><span class='glyphicon glyphicon-ok text-success'></span></button>";
var timeLogCancelButton = "<button type='button' class='btn btn-default btn-sm cancelTimeLog' title='cancel'><span class='glyphicon glyphicon-remove text-danger'></span></button>";
var timeLogEditButton = "<button type='button' class='btn btn-default btn-sm editTimeLog' title='edit'><span class='glyphicon glyphicon-pencil'></span></button>";

function initializeAttendance() {
    $('button.attendance').on('click', function (e) {
        var button = $(this);
        var eventId = $(this).closest('tr').data('eventid');
        var squadId = $(this).closest('tr').data('squadid');
        var playerId = $(this).closest('tr').data('playerid');
        var currentAttendanceId = $(this).closest('tr').data('attendanceid');
        var selectedAttendanceId = button.data('attendanceid');
        var finalAttendanceId = currentAttendanceId == selectedAttendanceId ? attendance.unSpecified : selectedAttendanceId;

        $.ajax({
            method: "POST",
            contentType: 'application/json',
            url: "/api/events/attendance",
            data: JSON.stringify({ "eventid": eventId, "squadid": squadId, "playerid": playerId, "attendance": finalAttendanceId })
        }).done(function (data) {
            var buttons = button.closest('tr').find("button.attendance");
            resetAttendanceButtons(buttons);

            if (finalAttendanceId != attendance.unSpecified) {
                var style =
                    (selectedAttendanceId == attendance.present ? "btn-success" :
                        (selectedAttendanceId == attendance.noShow ? "btn-danger" :
                            (selectedAttendanceId == attendance.late ? "btn-warning" : "btn-info")));

                $(button).addClass("attendance btn " + style);
            }

            if (finalAttendanceId == attendance.unSpecified || finalAttendanceId == attendance.noShow) {
                $(button).closest('tr').find("form.time-log").remove();
            } else if ($(button).closest('tr').find("form.time-log").length == 0) {
                var tableCell = $(button).closest('tr').find("td:nth-child(" + columns.timeLogged + ")");
                setUpTimeLogForm(tableCell);                
            }

            $(button).closest('tr').data('attendanceid', finalAttendanceId);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus);
        });
    });

    function resetAttendanceButtons(buttons) {
        for (var i = 0; i < buttons.length; i++) {
            $(buttons[i]).removeClass();
            $(buttons[i]).addClass("btn btn-default");
        }
    }

    function setUpTimeLogForm(tableCell) {
        var form = $(timeLogForm);
        var input = $(timeLogInput);
        var editButton = $(timeLogEditButton);

        form.append(input);
        form.append(editButton);
        attachEditTimeLogEvents(editButton);
        tableCell.append(form);
    }

    function attachEditTimeLogEvents(selector) {
        $(selector).unbind();
        $(selector).on('click', function (e) {            
            var form = $(this).closest('tr').find("form.time-log");            
            makeTimeLogFormEditable(form);           
        });        
    }

    function makeTimeLogFormEditable(form) {
        var input = form.find('input.time-input');
        input.removeAttr("readonly");
        var originalTimeLogged = input.val();
        var okButton = $(timeLogOkButton);
        var cancelButton = $(timeLogCancelButton);        

        $(cancelButton).on('click', function (e) {
            var form = $(this).closest('tr').find("form.time-log");
            var input = $(this).closest('tr').find('input.time-input');
            input.attr("readonly", "readonly");
            input.val(originalTimeLogged);
            resetTimeLogForm(form);
        });
        
        form.unbind();
        form.submit(function (e) {
            var submittedForm = $(this);
            e.preventDefault();            
            var eventId = $(submittedForm).closest('tr').data('eventid');
            var squadId = $(submittedForm).closest('tr').data('squadid');
            var playerId = $(submittedForm).closest('tr').data('playerid');
            var timelogged = $(submittedForm).find('input.time-input').val();

            $.ajax({
                method: "POST",
                contentType: 'application/json',
                url: "/api/events/timelog",
                data: JSON.stringify({ "eventid": eventId, "squadid": squadId, "playerid": playerId, "timelogged": timelogged })
            }).done(function (data) {               
                resetTimeLogForm(submittedForm);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                console.log(textStatus);
            });            
        });

        form.append(okButton);
        form.append(cancelButton);  
        form.find("button.editTimeLog").remove();
    }

    function resetTimeLogForm(form) {
        var input = form.find('input.time-input');
        input.attr("readonly", "readonly");   
        var editButton = $(timeLogEditButton);
        form.append(editButton);
        attachEditTimeLogEvents(editButton);
        form.find("button.saveTimeLog").remove();
        form.find("button.cancelTimeLog").remove();
    }
    
    attachEditTimeLogEvents('button.editTimeLog');
}