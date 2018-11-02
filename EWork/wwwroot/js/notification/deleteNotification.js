function deleteNotificationPost() {
    const token = $('input[name="__RequestVerificationToken"]').first().val();
    const notificationAmount = $('#numOfNotifications');
    const data = {
        'notificationId': $(this).data('notificationid'),
        '__RequestVerificationToken': token
    };
    const elem = this;
    $.post("/Notification/DeleteNotification",
        data,
        function () {
            $(elem).parent().parent().fadeOut();
            let newAmount = notificationAmount.html() - 1;
            if (newAmount < 0)
                newAmount = 0;
            notificationAmount.html(newAmount);
            if (newAmount === 0)
                notificationAmount.css('color', 'inherit');
        })
        .fail(function () {
            console.error("Can't delete a notification. Server error.");
        });
}

$(".delete-notification").click(deleteNotificationPost);
$('#delete-all-notifications').click(function () {
    $('.delete-notification').filter((idx, item) => $(item).parent().parent().css('display') !== 'none').click();
    if ($('#download-more-notifications').prop('disabled'))
        $(this).attr('disabled', 'disabled');
});