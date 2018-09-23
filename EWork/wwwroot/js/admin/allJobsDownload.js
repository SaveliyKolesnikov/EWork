$('#getMoreJobsButton').click(function () {
    const amountOfUsersNow = $('#jobsContainer tr').length;
    downloadJobs('/Admin/GetJobsAjax', takeAmount || 5, amountOfUsersNow, true);
});