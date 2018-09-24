$('#download-more-notifications').click(function () {
    downloadNotifications(takeAmount || 5);
});

function downloadNotifications(takeAmount) {
    let token = $('input[name="__RequestVerificationToken"]').first().val();
    let amountOfNotificationsNow = $('#notificationsTable>tr')
        .filter((idx, item) => $(item).css('display') !== 'none').length;
    let notificationReceiverUserName = $('input[name="notificationReceiver"]').val();
    return $.post('/Notification/GetNotifications',
        {
            '__RequestVerificationToken': token,
            'skipAmount': amountOfNotificationsNow,
            'takeAmount': takeAmount,
            'receiverUserName': notificationReceiverUserName
        },
        function (notifications) {
            notifications.forEach(addNotification);
            if (notifications.length < takeAmount)
                disableDownloadMoreNotificationsButton();
        }).fail(
            function (errorObj) {
                console.error(errorObj.responseJSON.message);
            });
}

function disableDownloadMoreNotificationsButton() {
    $('#download-more-notifications').unbind('click').prop({ disabled: true }).removeClass('blue');
}

function addNotification(notification) {
    let tr = $('<tr/>');

    tr.append($('<td/>', { text: notification.title }));

    let notificationSource = $('<a/>', { href: notification.source, text: 'link' });
    let sourceLinkTd = $('<td/>').append(notificationSource);
    tr.append(sourceLinkTd);

    let formattedDate = new Date(notification.createdDate);
    tr.append($('<td/>', { text: formattedDate.toLocaleString().replace(',', '') }));

    let deleteLink = $('<button/>', { text: 'Delete', class: 'delete-notification' })
        .data('notificationid', notification.id).addClass('delete-notification').click(deleteNotificationPost);
    tr.append($('<td/>').append(deleteLink));
    $('#notificationsTable').append(tr);
}