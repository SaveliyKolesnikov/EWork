// userName is declared in razor page.
const textInput = document.getElementById('messageText');
const sendDateInput = document.getElementById('sendDate');
const receiverInput = document.getElementById('receiverUserName');
const chat = document.getElementById('chat-history');
const messagesQueue = [];

if (typeof makeDialogActiveQuery !== 'undefined') {
    makeDialogActive($(makeDialogActiveQuery));
}

document.getElementById('submitMessageButton').addEventListener('click', function (e) {
    e.preventDefault();
    sendDateInput.value = new Date().format("m/dd/yyyy hh:MM TT");
    clearInputField();
    sendMessage();
});

function clearInputField() {
    let newMessage = new Message(new User(currentUserName, ""), new User(receiverInput.value, ""), textInput.value, new Date());
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

    let senderName = message.receiver.userName === currentUserName ? message.sender.userName : message.receiver.userName;
    if (receiverInput.value !== senderName) {
        let recMesBar = $(`.message-bar-elem[data-receiverusername='${senderName}']`);
        if (recMesBar.length === 0) {
            let newMessageBarElem = document.createElement('div');
            newMessageBarElem.innerHTML = `<div class="message-bar-elem row" data-receiverusername="${senderName}">
                                               <div class="row">
                                                   <div class="col-xs-12 col-md-4 text-center">
                                                       <img class="profile-photo" src="${message.sender.photoUrl}" alt="Profile photo" />
                                                   </div>
                                                   <div class="col-md-8 visible-md visible-lg visible-sm">
                                                       <div class="receiver-name">${message.sender.userName}</div>
                                                       <div class="message-preview">${message.text}</div>
                                                   </div>
                                               </div>
                                           </div>`;

            document.getElementById('message-bars-container').appendChild(newMessageBarElem);

            recMesBar = $(`.message-bar-elem[data-receiverusername='${senderName}']`);
            $(recMesBar).click(getMessages);
        }
        $(recMesBar)
            .css('order', `${maxOrder++}`)
            .addClass('new-message');
        $(".message-preview", recMesBar).html(message.text);
        return;
    }

    let isCurrentUserMessage = message.sender.userName === currentUserName;

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
    chat.scrollTop = chat.scrollHeight;
}

$('.message-bar-elem').click(getMessages);

function getMessages() {
    if (!makeDialogActive(this))
        return;
    chat.innerHTML = '<div class="text-center">Downloading messages</div>';

    let newReceiverUsername = receiverInput.value;
    let form = new FormData();
    let token = $('input[name="__RequestVerificationToken"]', $('#AntiForgeryToken')).val();
    form.append('__RequestVerificationToken', token);
    form.append('username1', newReceiverUsername);
    form.append('username2', currentUserName);

    fetch('/Chat/GetMessages', {
            method: "POST",
            credentials: 'include',
            body: form
        })
        .then(res => {
            res.json().then(messages => {
                $(chat).empty();
                for (let mes of messages) {
                    addMessageToChat(mes);
                }
            });
        });
}

function makeDialogActive(dialogElem) {
    if ($(dialogElem).hasClass('active'))
        return false;

    $('.message-bar-elem.active').removeClass('active').removeClass('new-message');
    $(dialogElem).addClass('active');

    receiverInput.value = $(dialogElem).data('receiverusername');

    return true;
}