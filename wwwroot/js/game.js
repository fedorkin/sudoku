"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("ReceiveCellValue", function (row, col, value, readonly) {
    var cellId = `${row},${col}`;
    var cell = document.getElementById(cellId);
    cell.value = value;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

var cells = document.getElementsByClassName("sudoku-cell");
Array.from(cells).forEach(function(element) {
    element.addEventListener('change', function(event){
        var cell = event.target;
        var index = cell.id.split(',');
        var row = parseInt(index[0]);
        var col = parseInt(index[1]);
        var value = parseInt(cell.value);
        var user = document.getElementById("userInput").value;
    
        connection.invoke("UpdateCellValue", row, col, value, user).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
  });