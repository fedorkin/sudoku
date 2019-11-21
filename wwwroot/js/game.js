"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

// Disable send button until connection is established
document.getElementById("signUpButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("ReceiveCellValue", function (row, col, value, userName) {
    var cellId = `${row},${col}`;
    var cell = document.getElementById(cellId);
    setValueToCell(cell, value, userName);
});

connection.on("CompettingCellIndexesException", function (row, col, compettingCellIndexes) {
    compettingCellIndexes.forEach(element => {
        var cellId = `${element.item1},${element.item2}`;
        var cell = document.getElementById(cellId);
        cell.classList.add("error");
    });
});

connection.on("DrawField", function (field) {
    fillTable(field);
    addInputLisenerToCells();
});

connection.start().then(function () {
    document.getElementById("signUpButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("signUpButton").addEventListener("click", function (event) {
    var name = document.getElementById("userInput").value;
    connection.invoke("SignedUser", name).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function addInputLisenerToCells()
{
    var cells = document.getElementsByClassName("sudoku-cell");
    Array.from(cells).forEach(function(element) {
        element.addEventListener('input', function(event){
            var cell = event.target;
            var index = cell.id.split(',');
            var row = parseInt(index[0]);
            var col = parseInt(index[1]);

            var value = parseInt(cell.value) || 0;

            connection.invoke("UpdateCellValue", row, col, value).catch(function (err) {
                return console.error(err.toString());
            });
            event.preventDefault();
        });
      });
}

function fillTable(field) {
    var cells = listToMatrix(field.cells, field.rank);
    var table = document.getElementById("field");
    for (var row = 0; row < field.rank; row++)
    {
        var tr = document.createElement('tr');
        
        for (var col = 0; col < field.rank; col++)
        {
            var fieldCell = cells[row][col];
            var td = document.createElement('td');

            var cell = document.createElement('input');
            cell.className = 'sudoku-cell';
            cell.id =`${row},${col}`;
            cell.setAttribute('type', 'number');

            setValueToCell(cell, fieldCell.value, fieldCell.userName);

            td.appendChild(cell);
            tr.appendChild(td);
        }

        table.appendChild(tr);
    }
}

function setValueToCell(cell, value, userName) {
    cell.setAttribute('user', userName);

    var currentUserName = document.getElementById("userInput").value;
    var editable = (value == 0) || (currentUserName == cell.userName);
    if (!editable)
    {
        cell.setAttribute('disabled', 'disabled');
    }

    var cellValue = value == 0 ? '' : value;
    cell.setAttribute('value', cellValue);
}

function listToMatrix(list, elementsPerSubArray) {
    var matrix = [], i, k;

    for (i = 0, k = -1; i < list.length; i++) {
        if (i % elementsPerSubArray === 0) {
            k++;
            matrix[k] = [];
        }

        matrix[k].push(list[i]);
    }

    return matrix;
}