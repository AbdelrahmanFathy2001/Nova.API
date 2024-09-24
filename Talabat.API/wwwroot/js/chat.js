
    var proxyConnection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

    proxyConnection.start().then(function(){
        document.getElementById("send").addEventListener("click", function(e) {
            e.preventDefault();
            proxyConnection.invoke("Send", recipient, message);
            console.log(`Sent message to ${recipient}: ${message}`);

        })
    }).catch((err) => {
        console.error(err);
    });

proxyConnection.on("ReceiveMessage", (senderId, message, connectionId) => {
        var liElement = document.createElement("li");
        liElement.innerHTML = `<strong>${senderId}: </strong> ${message}`;
        document.getElementById("con").appendChild(liElement);
});




//document.addEventListener('DOMContentLoaded', function () {
    
//    var messageInp = document.getElementById("messageInp");
//    var senderId = document.getElementById("senderId");
//    var ReceiverId = document.getElementById("ReceiverId");

//    var proxyConnection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

//    proxyConnection.start().then(function(){
//        document.getElementById("sendMessageBtn").addEventListener("click", function(e) {
//            e.preventDefault();
//            proxyConnection.invoke("Send", senderId, ReceiverId, messageInp.value);
//        })
//    }).catch((err) => {
//        console.error(err);
//    });

//    proxyConnection.on("ReceiveMessage", function (senderId, message) {
//        var liElement = document.createElement("li");
//        liElement.innerHTML = `<strong>${senderId}: </strong> ${message}`;
//        document.getElementById("con").appendChild(liElement);
//    })
//})