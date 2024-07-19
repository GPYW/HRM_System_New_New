
var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;


connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    console.log('connected to hub');
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;

    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
});   


toastr.options = {
    "closeButton": true,
    "progressBar": true,
    "showMethod": 'slideDown',
    "timeOut": 4000
};

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.textContent = `${user} says ${message}`;

    DisplayGeneralNotification(`${user}: ${message}`, 'General Message');
});

function DisplayGeneralNotification(message, title) {
    // Simply display the notification with the globally set options
    toastr.info(message, title);
}

connection.on("ReceivedPersonalNotification", function (user, message, username) {
    DisplayPersonalNotification(`${user}: ${message}`, 'Hey ' + username);
});

function DisplayPersonalNotification(message, title) {
    toastr.success(message, title);
}

connection.on("ReceivedGroupNotification", function (user, message, username) {
    DisplayPersonalNotification(`${user}: ${message}`, 'Hey ' + username);
});







    // Trigger Toastr notification on the client-side
    //toastr.options = {
    //    "closeButton": true,
    //    "debug": false,
    //    "newestOnTop": false,
    //    "progressBar": false,
    //    "positionClass": "toast-top-right",
    //    "timeOut": "5000",
    //    "extendedTimeOut": "1000",
    //    "onclick": null,
    //    "onHide": null,
    //    "onShow": null,
    //    "onHidden": null,
    //    "preventDuplicates": false,
    //    "preventOpenDuplicates": true,
    //    "tapToDismiss": false
    //};

    //toastr.success(`${user}: ${message}`);  // Customize toast type (success, info, warning, error)

    /*event.preventDefault();*/



//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});