$('#resetFiltersButton').click(function() {
    const filtersInputs = $('#filtersInputs input:not([type="button"])');
    filtersInputs.each((idx, elem) => $(elem).val(''));
});

function disableDownloadMoreJobsButton() {
    $('#download-more-jobs').unbind('click').prop({ disabled: true }).removeClass('blue');
}