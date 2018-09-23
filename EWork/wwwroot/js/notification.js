function newNotification() {
    const numOfNotifications = document.getElementById('numOfNotifications');
    let currentNumOfNotifications = parseInt(numOfNotifications.innerHTML);
    numOfNotifications.innerHTML = ++currentNumOfNotifications;

    const animationDuration = 250;
    $(numOfNotifications).animate({
        color: "white"
    }, animationDuration, function () { $(this).animate({ color: "red" }, animationDuration); });
}