$('#download-more-jobs').click(function () {
    const amountOfJobsNow = $('.job-container').length;
    if (!takeAmount)
        takeAmount = 5;
    downloadJobs(takeAmount, amountOfJobsNow);
});

function downloadJobs(takeAmount, skipAmount) {
    const token = $('input[name="__RequestVerificationToken"]', $('#antiForgeryToken')).val();
    const requiredTags = $('#SearchingByTags').val();
    const employerRatingFrom = $('input[name="employerRatingFrom"]').val();
    const employerRatingTo = $('input[name="employerRatingTo"]').val();
    const budgetTo = $('input[name="budgetTo"]').val();
    const budgetFrom = $('input[name="budgetFrom"]').val();
    const getJobsAjaxMethod = $('input[name="getJobsAjaxMethodUrl"]').val();

    $.post(getJobsAjaxMethod,
        {
            '__RequestVerificationToken': token,
            'skipAmount': skipAmount,
            'takeAmount': takeAmount,
            'requiredTags': requiredTags,
            'employerRatingFrom': employerRatingFrom,
            'employerRatingTo': employerRatingTo,
            'budgetFrom': budgetFrom,
            'budgetTo': budgetTo
        },
        function (jobs) {
            jobs.forEach(addJob);
            if (jobs.length < takeAmount) {
                // Disable the download more jobs button.
                $('#download-more-jobs').unbind('click').prop({ disabled: true }).removeClass('blue');
            }
        }).fail(
            function (errorObj) {
                console.error(errorObj.responseJSON.message);
            });
}

function addJob(job) {
    const container = $('<div/>', { class: 'job-container' });
    const tags = [];

    let creationDate = new Date(job.creationDate);
    creationDate = removeSecondsFromDateString(creationDate.toLocaleString());
    for (let tag of job.tags) {
        tags.push($('<div/>', { class: 'job-tag' }).text(tag).click(function () { findJobsByTag(this); }));
    }
    container.html(`<h4 class="job-title">
                        <a href="/Job/JobInfo?jobid=${job.id}">${job.title}</a>
                    </h4>
                    <h5 class="posted-date">
                        Posted <span>${creationDate}</span>
                    </h5>
                    <div class="space-10"></div>
                    <div class="job-budget">
                        ${job.budget}$ <br />
                    </div>
                    <p class="fixed-price">Fixed price</p>
                    <div class="job-description">
                        ${job.description}
                    </div>
                    <div class="space-30"></div>
                    <div class="job-tags">
                    </div>
                    <div class="space-20"></div>
                    <div class="user-rating">Employer rating: <span>${job.employerRating}</span></div>`);
    const tagsContainer = $('.job-tags', container);
    tags.forEach(tag => tagsContainer.append(tag));
    $('#jobs-container').append(container);

    if (job.employerUserName === currentUserUserName && job.hiredFreelancerUserName === "") {
        const deleteJobForm = `<form type="post" action="/DeleteJob">
                                <input type="hidden" name="jobId" value="${job.id}" />
                                <input type="submit" class="close-job" value="&Chi;" />
                            </form>`;
        container.append(deleteJobForm);
    }

    function removeSecondsFromDateString(dateString) {
        const lastColonIndex = dateString.lastIndexOf(':');
        let firstWhitespaceAfterLastColon = dateString.indexOf(' ', lastColonIndex);
        if (firstWhitespaceAfterLastColon === -1)
            firstWhitespaceAfterLastColon = dateString.length;

        return dateString.slice(0, lastColonIndex) + dateString.slice(firstWhitespaceAfterLastColon);
    }
}