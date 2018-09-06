const numOfNotifications = document.getElementById('numOfNotifications');

function newNotification() {
    let currentNum = parseInt(numOfNotifications.innerHTML);

    if (currentNum === 0)
        numOfNotifications.color = "red";

    numOfNotifications.innerHTML = ++currentNum;
}