$('#confirmBlockingModalCenter').on('show.bs.modal',
    function (event) {
        const button = $(event.relatedTarget); // element that triggered the modal
        const action = button.data('text');
        const modal = $(this);
        modal.find('.modal-title').text(action);
        modal.find('.modal-body').text(`Are you sure you want to ${action}?`);
        modal.find('input[name="blockedUserUserName"]').val(button.data('username'));
    });

$('#confirmBlockingModalCenter .confirm-button').click(function (e) {
    e.preventDefault();
    const userName = $(this).parent().find('input[name="blockedUserUserName"]').val();
    $(`#blockUserSubmit[data-username="${userName}"]`).click();
});

$('#deleteUserModal').on('show.bs.modal',
    function (event) {
        const button = $(event.relatedTarget); // element that triggered the modal
        const recipient = button.data('username');
        const modal = $(this);
        modal.find('.modal-title').text(`Delete ${recipient}`);
        modal.find('.modal-body').text(`Are you sure you want to delete ${recipient}?`);
        modal.find('input[name="deletedUserUserName"]').val(button.data('username'));
    });

$('#deleteUserModal .confirm-button').click(function (e) {
    e.preventDefault();
    const userName = $(this).parent().find('input[name="blockedUserUserName"]').val();
    $(`#deleteUserSubmit[data-username="${userName}"]`).click();
});

$('#replenishBalanceModal').on('show.bs.modal',
    function (event) {
        const button = $(event.relatedTarget); // element that triggered the modal
        const recipient = button.data('username');
        const modal = $(this);
        modal.find('.modal-title').text(`Replenish a ${recipient} balance`);
        modal.find('input[name="balanceid"]').val(button.parent().data('balanceid'));
    });

$('#replenishBalanceModal .confirm-button').click(function (e) {
    e.preventDefault();
    let amountOfReplenishment = $('#amountOfReplenishment').val();
    if (!amountOfReplenishment.trim())
        amountOfReplenishment = 0;

    const balanceId = $(this).parent().find('input[name="balanceid"]').val();
    const targetForm = $(`.replenish-balance-form[data-balanceid="${balanceId}"]`);
    $('#amountOfReplenishmentInput', targetForm).val(amountOfReplenishment);
    $('#replenishBalanceSubmit', targetForm).click();
});

document.querySelector('#amountOfReplenishment').addEventListener("keyup", function (event) {
    event.preventDefault();

    // Number 13 is the "Enter" key on the keyboard
    if (event.keyCode === 13) {
        // Trigger the button element with a click
        $('#replenishBalanceModal .confirm-button').click();
    }
});

$('#decreaseBalanceModal').on('show.bs.modal',
    function (event) {
        const button = $(event.relatedTarget); // element that triggered the modal
        const recipient = button.data('username');
        const modal = $(this);
        modal.find('.modal-title').text(`Decrease a ${recipient} balance`);
        modal.find('input[name="balanceId"]').val(button.parent().data('balanceid'));
    });

$('#decreaseBalanceModal .confirm-button').click(function (e) {
    e.preventDefault();
    let amountOfReplenishment = $('#decreaseAmount').val();
    if (!amountOfReplenishment.trim())
        amountOfReplenishment = 0;

    const balanceId = $(this).parent().find('input[name="balanceId"]').val();
    const targetForm = $(`.decrease-balance-form[data-balanceid="${balanceId}"]`);
    $('#decreaseAmountInput', targetForm).val(amountOfReplenishment);
    $('#decreaseBalanceSubmit', targetForm).click();
});

function refreshEventListeners() {
    document.querySelector('#decreaseAmount').addEventListener("keyup", function (event) {
        event.preventDefault();

        // Number 13 is the "Enter" key on the keyboard
        if (event.keyCode === 13) {
            // Trigger the button element with a click
            $('#decreaseBalanceModal .confirm-button').click();
        }
    });

    $('.disable-submitting-on-enter').on('keyup keypress', function (e) {
        const keyCode = e.keyCode || e.which;
        if (keyCode === 13) {
            e.preventDefault();
            return false;
        }
    });
}

refreshEventListeners();