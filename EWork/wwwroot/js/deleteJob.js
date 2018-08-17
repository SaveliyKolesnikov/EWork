$(".deleteJobLink").click(deleteJob);

function deleteJob() {
    let deletedJobId = $(this).data("job-id");
    let deletedElement = $(this).parent().parent();
    $.post('DeleteJob',
            { jobId: deletedJobId })
        .done(() => deletedElement.remove());
}