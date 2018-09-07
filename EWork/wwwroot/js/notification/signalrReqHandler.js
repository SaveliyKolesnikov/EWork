var connection = new signalR.HubConnectionBuilder()
    .withUrl('/Hubs/Notification')
    .build();

connection.on('newNotification', newNotification);

connection.start()
    .catch(error => {
        console.error(error.message);
    });