$('.job-tag').click(function() { findJobsByTag(this.innerHTML) });

function findJobsByTag(tagValue = "") {
    let url = `/Job/JobBoard?RequiredTags=${tagValue}`;
    window.location.replace(url);
}