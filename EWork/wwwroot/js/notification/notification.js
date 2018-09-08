const numOfNotifications = document.getElementById('numOfNotifications');

function newNotification() {
    let currentNum = parseInt(numOfNotifications.innerHTML);
    numOfNotifications.innerHTML = ++currentNum;

    const animationDuration = 250;
    $(numOfNotifications).animate({
        color: "white"
    }, animationDuration, function () { $(this).animate({ color: "red" }, animationDuration); });
}