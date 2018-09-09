function newNotification() {
    let numOfNotifications = document.getElementById('numOfNotifications');
    let currentNum = parseInt(numOfNotifications.innerHTML);
    numOfNotifications.innerHTML = ++currentNum;

    const animationDuration = 250;
    $(numOfNotifications).animate({
        color: "white"
    }, animationDuration, function () { $(this).animate({ color: "red" }, animationDuration); });
}