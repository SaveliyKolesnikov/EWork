var connection = new signalR.HubConnectionBuilder()
    .withUrl('/EWorkChat')
    .build();

connection.on('receiveMessage', addMessageToChat);

connection.start()
    .catch(error => {
        console.error(error.message);
    });

function sendMessageToHub(message) {
    connection.invoke('sendMessage', message.senderUserName, message.receiverUserName, message.text);
}