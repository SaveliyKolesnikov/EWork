$('.autodeletion-button').click(function() {
    if ($('.autodeletion-checkbox').is(':checked'))
        DisableNotificationsAutodeletion();
    else
        EnableNotificationsAutodeletion();
});

function DisableNotificationsAutodeletion() {
    $('.autodeletion-checkbox').attr('checked', false);
    $('.autodeletion-button').removeClass('blue').html('OFF');
}

function EnableNotificationsAutodeletion() {
    $('.autodeletion-checkbox').attr('checked', true);
    $('.autodeletion-button').addClass('blue').html('ON');

}