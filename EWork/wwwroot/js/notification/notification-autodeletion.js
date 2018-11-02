$('.autodeletion-button').click(function() {
    if ($('.autodeletion-checkbox').is(':checked'))
        disableNotificationsAutodeletion();
    else
        enableNotificationsAutodeletion();
});

function disableNotificationsAutodeletion() {
    $('.autodeletion-checkbox').attr('checked', false);
    $('.autodeletion-button').removeClass('blue').html('OFF');
}

function enableNotificationsAutodeletion() {
    $('.autodeletion-checkbox').attr('checked', true);
    $('.autodeletion-button').addClass('blue').html('ON');
}

function onSourceLinkClick(e) {
    if (!$('.autodeletion-checkbox').is(':checked'))
        return;

    const deleteNotificationLink = $('.delete-notification', $(e.target).parent().parent());
    deleteNotificationLink.click();
}

