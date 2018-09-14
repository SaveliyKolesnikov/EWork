$('#getMoreJobsButton').click(function () {
    const takeAmount = 5;
    const amountOfUsersNow = $('#jobsContainer tr').length;
    downloadJobs('/Admin/GetJobsAjax', takeAmount, amountOfUsersNow, true);
});