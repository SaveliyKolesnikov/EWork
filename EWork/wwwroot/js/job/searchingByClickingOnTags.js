$('.job-tag').click(function() { findJobsByTag(this) });

function findJobsByTag(tagElem) {
    let searchUrl = $('input[name="searchUrl"]').val();
    let url = `${searchUrl}?RequiredTags=${tagElem.innerHTML}`;
    window.location.replace(url);
}