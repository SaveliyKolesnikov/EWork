$('#getMoreUsersButton').click(function () {
    const takeAmount = 5;
    const amountOfUsersNow = $('#usersContainer tr').length;
    downloadJobs(takeAmount, amountOfUsersNow);
});

function downloadJobs(takeAmount, skipAmount) {
    const token = $('input[name="__RequestVerificationToken"]').first().val();
    const searchString = $('#SearchString').val();

    $.post('/Admin/GetUsersAjax',
        {
            '__RequestVerificationToken': token,
            'skipAmount': skipAmount,
            'takeAmount': takeAmount,
            'searchString': searchString
        },
        function (users) {
            users.forEach(addUserToTable);
        }).fail(
        function (errorObj) {
            console.error(errorObj.responseJSON.message);
        });
}

function addUserToTable(user) {
    const userRow = document.createElement('tr');
    const token = $('input[name="__RequestVerificationToken"]').first().val();
    const checkedAttr = user.isBlocked ? `checked="checked"` : '';
    const jobElements = [];
    for (let jobId of user.jobs) {
        const elem = $('<a/>', { href: `/Job/JobInfo?jobid=${jobId}`, text: jobId });
        jobElements.push(elem);
    }
    userRow.innerHTML = `
        <td>
            <a href="/Profile/Profile?username=${user.userName}">${user.userName}</a>
        </td >
        <td>
            ${user.role}
        </td>
        <td>
            ${user.fullName}
        </td>
        <td>
            ${new Date(user.signUpDate).toLocaleString().replace(',','')}
        </td>
        <td>
            <rating>Rating: ${user.rating}</rating>
        </td>
        <td class="jobs-ids">
        </td>
        <td>
            $${user.money}
        </td>
        <td>
            <input class="check-box" ${checkedAttr} disabled="disabled" type="checkbox"/>
        </td>
        <td>
            <form action="/Admin/BlockUser" method="post">
                <a style="cursor: pointer;" data-toggle="modal" data-target="#confirmBlockingModalCenter" data-username="${user.userName}" data-text="Block ${user.userName}">
                    Block
                </a><br />
                <input type="hidden" name="userId" value="${user.id}" />
                <input id="blockUserSubmit" data-username="${user.userName}" type="submit" style="display: none;" />
                <input name="__RequestVerificationToken" type="hidden" value="${token}" /></form>
            <form action="/Admin/DeleteUser" method="post">
                <a style="cursor: pointer;" data-toggle="modal" data-target="#deleteUserModal" data-username="${user.userName}">
                    Delete
                </a><br />
                <input type="hidden" name="userId" value="${user.id}" />
                <input id="deleteUserSubmit" data-username="${user.userName}" type="submit" style="display: none;" />

                <input name="__RequestVerificationToken" type="hidden" value="${token}" /></form>
            <form class="replenish-balance-form" data-balanceid="${user.balanceId}" action="/Admin/ReplenishBalance" method="post">
                <a style="cursor: pointer;" data-toggle="modal" data-target="#replenishBalanceModal">
                    Replenish Balance
                            </a><br />

                <input type="hidden" name="balanceId" value="${user.balanceId}" />
                <input id="amountOfReplenishmentInput" type="hidden" name="amount" value="0" />
                <input id="replenishBalanceSubmit" type="submit" style="display: none;" />

                <input name="__RequestVerificationToken" type="hidden" value="${token}" /></form>
            <form class="decrease-balance-form" data-balanceid="${user.balanceId}" action="/Admin/DecreaseBalance" method="post">
                <a style="cursor: pointer;" data-toggle="modal" data-target="#decreaseBalanceModal">
                    Decrease Balance
                            </a><br />

                <input type="hidden" name="balanceId" value="${user.balanceId}" />
                <input id="decreaseAmountInput" type="hidden" name="amount" value="0" />
                <input id="decreaseBalanceSubmit" type="submit" style="display: none;" />

                <input name="__RequestVerificationToken" type="hidden" value="${token}" /></form>
        </td>`;
    const jobsIdsElem = $('.jobs-ids', $(userRow));
    jobElements.forEach(job => jobsIdsElem.append(job));
    document.getElementById('usersContainer').appendChild(userRow);
    refreshEventListeners();
}