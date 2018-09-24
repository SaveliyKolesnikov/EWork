function downloadJobs(postUrl, takeAmount, skipAmount, isActionsRequired = false, noMoreJobsAction = () => { }) {
    const token = $('input[name="__RequestVerificationToken"]').first().val();
    const searchString = $('#SearchString').val();

    $.post(postUrl,
        {
            '__RequestVerificationToken': token,
            'skipAmount': skipAmount,
            'takeAmount': takeAmount,
            'searchString': searchString
        },
        function (jobs) {
            jobs.forEach(j => addJobToTable(j, isActionsRequired));
            if (jobs.length < takeAmount)
                noMoreJobsAction();
        }).fail(
            function (errorObj) {
                console.error(errorObj.responseJSON.message);
            });
}

function addJobToTable(job, isActionsRequired = false) {
    const jobRow = $('<tr/>');
    jobRow.append($('<td/>', { text: job.id }));
    jobRow.append($('<td/>').append($('<a/>', { href: `/Job/JobInfo?jobid=${job.id}`, text: job.title })));
    jobRow.append($('<td/>', { text: new Date(job.creationDate).toLocaleString().replace(',', '') }));

    const employerProfileTd = $('<td/>');
    employerProfileTd.append($('<a/>',
        { href: `/Profile/Profile?username=${job.employerUserName}`, text: job.employerUserName }));
    employerProfileTd.append($('<rating/>', { text: ` (Rating: ${job.employerRating})` }));
    jobRow.append(employerProfileTd);

    const freelancerProfileTd = $('<td/>');
    freelancerProfileTd.append($('<a/>',
        { href: `/Profile/Profile?username=${job.hiredFreelancerUserName}`, text: job.hiredFreelancerUserName }));
    if (job.hiredFreelancerUserName)
        freelancerProfileTd.append($('<rating/>', { text: ` (Rating: ${job.hiredFreelancerRating})` }));
    jobRow.append(freelancerProfileTd);

    if (isActionsRequired) {
        const token = $('input[name="__RequestVerificationToken"]').first();
        const actionCell = $('<td/>');
        const actionForm = $('<form/>', { action: '/Job/DeleteJob', method: 'post' });
        actionForm.append($('<input/>', { name: 'jobid', value: job.id, type: 'hidden' }));
        actionForm.append(token);
        actionForm.append($('<input/>', { class: 'delete-job-input', type: 'submit', value: 'Delete' }));

        actionCell.append(actionForm);
        jobRow.append(actionCell);
    }

    $('#jobsContainer').append(jobRow);
}