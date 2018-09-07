var connection = new signalR.HubConnectionBuilder()
    .withUrl('/Hubs/EWorkChat')
    .build();

connection.on('receiveMessage', addMessageToChat);

connection.start()
    .catch(error => {
        console.error(error.message);
    });

function sendMessageToHub(message) {
    connection.invoke('sendMessage', message);
}