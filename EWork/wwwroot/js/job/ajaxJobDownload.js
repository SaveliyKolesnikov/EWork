$('#download-more-notifications').click(function() {
    const takeAmount = 5;
    downloadJobs(takeAmount);
});

function downloadJobs(takeAmount) {
    let token = $('input[name="__RequestVerificationToken"]', $('#antiForgeryToken')).val();
    let amountOfJobsNow = $('.job-container').length;
    let requiredTags = $('#SearchingByTags').val();
    let getJobsAjaxMethod = $('input[name="getJobsAjaxMethodUrl"]').val();

    $.post(getJobsAjaxMethod,
        {
            '__RequestVerificationToken': token,
            'skipAmount': amountOfJobsNow,
            'takeAmount': takeAmount,
            'requiredTags': requiredTags
        },
        function (jobs) {
            jobs.forEach(addJob);
        }).fail(
        function (errorObj) {
            console.error(errorObj.responseJSON.message);
        });
}