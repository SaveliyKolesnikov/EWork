$('#confirmBlockingModalCenter').on('show.bs.modal',
    function (event) {
        const button = $(event.relatedTarget); // element that triggered the modal
        const action = button.data('text');
        const modal = $(this);
        modal.find('.modal-title').text(action);
        modal.find('.modal-body').text(`Are you sure you want to ${action}?`);
    });

$('#confirmBlockingModalCenter .confirm-button').click(function (e) {
    e.preventDefault();
    $('#blockUserSubmit').click();
});

$('#deleteUserModal').on('show.bs.modal',
    function (event) {
        const button = $(event.relatedTarget); // element that triggered the modal
        const recipient = button.data('username');
        const modal = $(this);
        modal.find('.modal-title').text(`Delete ${recipient}`);
        modal.find('.modal-body').text(`Are you sure you want to delete ${recipient}?`);
    });

$('#deleteUserModal .confirm-button').click(function (e) {
    e.preventDefault();
    $('#deleteUserSubmit').click();
});

$('#replenishBalanceModal').on('show.bs.modal',
    function (event) {
        const button = $(event.relatedTarget); // element that triggered the modal
        const recipient = button.data('username');
        const modal = $(this);
        modal.find('.modal-title').text(`Replenish a ${recipient} balance`);
    });

$('#replenishBalanceModal .confirm-button').click(function (e) {
    e.preventDefault();
    let amountOfReplenishment = $('#amountOfReplenishment').val();
    if (!amountOfReplenishment.trim())
        amountOfReplenishment = 0;

    $('#amountOfReplenishmentInput').val(amountOfReplenishment);
    $('#replenishBalanceSubmit').click();
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
    });

$('#decreaseBalanceModal .confirm-button').click(function (e) {
    e.preventDefault();
    let amountOfReplenishment = $('#decreaseAmount').val();
    if (!amountOfReplenishment.trim())
        amountOfReplenishment = 0;

    $('#decreaseAmountInput').val(amountOfReplenishment);
    $('#decreaseBalanceSubmit').click();
});

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