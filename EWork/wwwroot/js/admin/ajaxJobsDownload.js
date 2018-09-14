$('#getMoreJobsButton').click(function () {
    const takeAmount = 5;
    const amountOfUsersNow = $('#jobsContainer tr').length;
    downloadJobs(takeAmount, amountOfUsersNow);
});

function downloadJobs(takeAmount, skipAmount) {
    const token = $('input[name="__RequestVerificationToken"]').first().val();
    const searchString = $('#SearchString').val();

    $.post('/Admin/GetJobsAjax',
        {
            '__RequestVerificationToken': token,
            'skipAmount': skipAmount,
            'takeAmount': takeAmount,
            'searchString': searchString
        },
        function (jobs) {
            jobs.forEach(addJobToTable);
        }).fail(
            function (errorObj) {
                console.error(errorObj.responseJSON.message);
            });
}

function addJobToTable(job) {
    const jobRow = $('<tr/>');
    jobRow.append($('<td/>', { text: job.id }));
    jobRow.append($('<td/>').append($('<a/>', { href: `/Job/JobInfo?jobid=${job.id}`, text: job.title })));
    jobRow.append($('<td/>', { text: new Date(job.createdDate).toLocaleString().replace(',', '') }));

    const employerProfileTd = $('<td/>');
    employerProfileTd.append($('<a/>',
        { href: `/Profile/Profile?username=${job.employerUserName}`, text: job.employerUserName }));
    employerProfileTd.append($('<rating/>', { text: `(Rating: ${job.employerRating})` }));
    jobRow.append(employerProfileTd);

    const freelancerProfileTd = $('<td/>');
    freelancerProfileTd.append($('<a/>',
        { href: `/Profile/Profile?username=${job.hiredFreelancerUserName}`, text: job.hiredFreelancerUserName }));
    if (job.hiredFreelancerUserName)
        freelancerProfileTd.append($('<rating/>', { text: `(Rating: ${job.hiredFreelancerRating})` }));
    jobRow.append(freelancerProfileTd);

    jobRow.append($('<a/>', { href: `/Job/DeleteJob?jobid=${job.id}`, text: 'Delete' }));
    $('#jobsContainer').append(jobRow);
}