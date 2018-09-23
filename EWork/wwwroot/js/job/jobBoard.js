$('#resetFiltersButton').click(function() {
    const filtersInputs = $('#filtersInputs input:not([type="button"])');
    filtersInputs.each((idx, elem) => $(elem).val(''));
});