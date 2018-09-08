let token = $('input[name="__RequestVerificationToken"]', $('#antiForgeryToken')).val();
function deleteNotificationPost() {
    let data = {
        'notificationId': $(this).data('notificationid'),
        '__RequestVerificationToken': token
    };
    let elem = this;
    $.post("/Notification/DeleteNotification",
        data,
        function () {
            $(elem).parent().parent().fadeOut();
        })
        .fail(function () {
            alert("Can't delete a notification. Server error.");
        });
}

$(".delete-notification").click(deleteNotificationPost);
$('#delete-all-notifications').click(() => $('.delete-notification')
    .filter(() => $(this).parent().parent().css('display') !== 'none').click());