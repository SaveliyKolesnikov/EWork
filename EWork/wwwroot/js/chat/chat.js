// userName is declared in razor page.
const textInput = document.getElementById('messageText');
const sendDateInput = document.getElementById('sendDate');
const receiverInput = document.getElementById('receiverUserName');
const chat = document.getElementById('chat-history');
const messagesQueue = [];

document.getElementById('submitMessageButton').addEventListener('click', function (e) {
    e.preventDefault();
    sendDateInput.value = new Date().format("m/dd/yyyy hh:MM TT");
    clearInputField();
    sendMessage();
});

function clearInputField() {
    let newMessage = new Message(senderUserName.value, receiverInput.value, textInput.value, new Date());
    messagesQueue.push(newMessage);
    textInput.value = "";
}

function sendMessage() {
    let message = messagesQueue.shift() || "";
    if (message === "" || message.text.trim() === "")
        return;

    sendMessageToHub(message);
}

function addMessageToChat(message) {
    //    if (currentReceiver !== message.receiverUserName && currentReceiver !== message.senderUserName) 
    //      return;

    let senderName = message.receiverUserName === currentUserName ? message.senderUserName : message.receiverUserName;
    if (receiverInput.value !== senderName) {
        let recMesBar = $(`.message-bar-elem[data-receiverusername='${senderName}']`);
        $(recMesBar)
            .css('order', `${maxOrder++}`)
            .addClass('new-message');
        $(".message-preview", recMesBar).html(message.text);
        return;
    }

    let isCurrentUserMessage = message.senderUserName === currentUserName;

    let container = document.createElement('div');
    container.className = isCurrentUserMessage ? "container darker" : "container";

    let text = document.createElement('p');
    text.innerHTML = message.text;

    let when = document.createElement('span');
    when.className = isCurrentUserMessage ? "time-left" : "time-right";
    when.innerHTML = new Date(message.sendDate).format("m/dd/yyyy hh:MM TT");

    container.appendChild(text);
    container.appendChild(when);
    chat.appendChild(container);
}
