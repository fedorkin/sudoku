"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

// Disable send button until connection is established
document.getElementById("signUpButton").disabled = true;

connection.on("DrawField", function (field) {
    if (!document.getElementById("field")) {
        createTable(field);
    }

    fillTable(field);
});

connection.start().then(function () {
    document.getElementById("signUpButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveCellValue", function (row, col, fieldCell) {
    var cellId = getCellId(row, col);
    var inputCell = document.getElementById(cellId);
    fillInputCell(inputCell, fieldCell);
});

document.getElementById("signUpButton").addEventListener("click", function (event) {
    var name = document.getElementById("userInput").value;
    connection.invoke("SignedUser", name).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function createTable(field) {
    var tableContainer = document.getElementById("field-container")
    var table = document.createElement("table");
    table.id = "field";

    for (var row = 0; row < field.rank; row++)
    {
        var tr = document.createElement('tr');
        
        for (var col = 0; col < field.rank; col++)
        {
            var td = document.createElement('td');

            var inputCell = document.createElement('input');
            inputCell.className = 'sudoku-cell';
            inputCell.id = getCellId(row, col);
            inputCell.setAttribute('type', 'number');
            inputCell.setAttribute('min', '0');
            inputCell.setAttribute('max', '9');

            inputCell.addEventListener('input', inputCellHandler);

            td.appendChild(inputCell);
            tr.appendChild(td);
        }

        table.appendChild(tr);
    }

    tableContainer.appendChild(table);
}

function fillTable(field) {
    var cells = listToMatrix(field.cells, field.rank);
    for (var row = 0; row < field.rank; row++)
    {
        for (var col = 0; col < field.rank; col++)
        {
            var fieldCell = cells[row][col];
            var cellId = getCellId(row, col);
            var inputCell = document.getElementById(cellId);

            fillInputCell(inputCell, fieldCell);
        }
    }
}

function inputCellHandler(event) {
    var inputCell = event.target;
    var index = inputCell.id.split(',');
    var row = parseInt(index[0]);
    var col = parseInt(index[1]);

    var value = parseInt(inputCell.value) || 0;

    connection.invoke("UpdateCellValue", row, col, value).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
};

function getCellId(row, col) {
    return `${row},${col}`;
}

function fillInputCell(inputCell, fieldCell) {
    inputCell.setAttribute('user', fieldCell.userName);
    inputCell.disabled = !fieldCell.editable;
    
    if (fieldCell.isCompeting) {
        inputCell.classList.add("competting");
    } else {
        inputCell.classList.remove("competting");
    }

    var cellValue = fieldCell.value == 0 ? '' : fieldCell.value;
    inputCell.value = cellValue;
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