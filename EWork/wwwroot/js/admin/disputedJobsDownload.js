$('#getMoreJobsButton').click(function () {
    const amountOfUsersNow = $('#jobsContainer tr').length;
    downloadJobs('/Admin/GetDisputedJobsAjax', takeAmount || 5, amountOfUsersNow, false, () => this.disabled = true);
});