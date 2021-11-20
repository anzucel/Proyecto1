var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

connection.on("ReceiveMessage", (username, message) => {
    const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    const date = new Date().toLocaleTimeString();
    const ShowMesage = " <strong>" + username + "</strong>:" + msg + "<br>" + date ;
    const li = document.createElement("li");
    li.innerHTML = ShowMesage;
    document.getElementById("messagesList").appendChild(li);
    //const p = document.createElement("p");
    //p.innerHTML = ShowMesage;
    //document.getElementById("discussion").appendChild(p);
});

connection.start().catch(err => {
    console.error(err.toString());
});


document.getElementById("sendButton").addEventListener("click", event => {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
    event.preventDefault();
});