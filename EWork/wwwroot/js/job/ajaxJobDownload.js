$('#download-more-notifications').click(function () {
    const takeAmount = 2;
    const amountOfJobsNow = $('.job-container').length;
    downloadJobs(takeAmount, amountOfJobsNow);
});

function downloadJobs(takeAmount, skipAmount) {
    const token = $('input[name="__RequestVerificationToken"]', $('#antiForgeryToken')).val();
    const requiredTags = $('#SearchingByTags').val();
    const getJobsAjaxMethod = $('input[name="getJobsAjaxMethodUrl"]').val();

    $.post(getJobsAjaxMethod,
        {
            '__RequestVerificationToken': token,
            'skipAmount': skipAmount,
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

    function removeSecondsFromDateString(dateString) {
        const lastColonIndex = dateString.lastIndexOf(':');
        let firstWhitespaceAfterLastColon = dateString.indexOf(' ', lastColonIndex);
        if (firstWhitespaceAfterLastColon === -1)
            firstWhitespaceAfterLastColon = dateString.length;

        return dateString.slice(0, lastColonIndex) + dateString.slice(firstWhitespaceAfterLastColon);
    }
}