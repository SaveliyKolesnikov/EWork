$('.job-tag').click(function() { findJobsByTag(this) });

function findJobsByTag(tagElem) {
    let url = `${tagElem.dataset.searchurl}?RequiredTags=${tagElem.innerHTML}`;
    window.location.replace(url);
}