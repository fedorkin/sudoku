"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

document.getElementById("signInButton").disabled = true;

connection.start().then(function () {
    document.getElementById("signInButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("DrawField", function (field) {
    if (!document.getElementById("field")) {
        createTable(field);
    }

    fillTable(field);
});

connection.on("ReceivePlayers", function(players) {
    recreatePlayersTable(players);
    recreatePlayersStyles(players);
});

connection.on("ReceiveCellValue", function (row, col, fieldCell) {
    var cellId = getCellId(row, col);
    var inputCell = document.getElementById(cellId);
    fillInputCell(inputCell, fieldCell);
});

document.getElementById("signInButton").addEventListener("click", function (event) {
    var playerInput = document.getElementById("playerInput");
    playerInput.disabled = true;
    
    connection.invoke("SignedPlayer", playerInput.value).catch(function (err) {
        playerInput.disabled = false;
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("newGameButton").addEventListener("click", function (event) {
    connection.invoke("NewGame").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function recreatePlayersTable(players) {
    var tableContainer = document.getElementById("topScores-container")

    var table = document.getElementById("topScoresTable");
    if (table != null) {
        tableContainer.removeChild(topScoresTable);
    }

    table = document.createElement("table");
    table.className = "players-table";
    table.id = "topScoresTable";

    var header = table.createTHead();
    var row = header.insertRow();

    var th = document.createElement("th");
    var text = document.createTextNode("player");
    th.appendChild(text);
    row.appendChild(th);

    th = document.createElement("th");
    text = document.createTextNode("score");
    th.appendChild(text);
    row.appendChild(th);

    players.forEach(player => {
        row = table.insertRow();

        var cell = row.insertCell();
        var text = document.createTextNode(player.name);
        cell.setAttribute('player', player.name);
        cell.appendChild(text);

        cell = row.insertCell();
        text = document.createTextNode(player.scores);
        cell.appendChild(text);
    });

    tableContainer.appendChild(table);
}

function recreatePlayersStyles(players) {
    var styleSheet = document.getElementById("userColors");
    if (styleSheet) {
        document.head.removeChild(styleSheet);
    }

    styleSheet = document.createElement("style")
    var currentPlayerName = document.getElementById("playerInput").value;

    var styles = "";
    players.forEach(player => {
        var background = currentPlayerName === player.name ? "white" : intToRGB(hashCode(player.connectionId));
        var color = currentPlayerName === player.name ? "black" : "white";
        var playerStyle = `[player="${player.name}"] { background: ${background}; color: ${color}; }\n`;
        styles += playerStyle;
    });

    styleSheet.id = "userColors"
    styleSheet.type = "text/css"
    styleSheet.innerText = styles
    document.head.appendChild(styleSheet)
}

function createTable(field) {
    var tableContainer = document.getElementById("field-container")
    var table = document.createElement("table");
    table.id = "field";
    table.className = "sudoku-field";

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
            inputCell.setAttribute('max', field.rank);

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
    inputCell.setAttribute('player', fieldCell.playerName);
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

function hashCode(str) {
    var hash = 0;
    for (var i = 0; i < str.length; i++) {
       hash = str.charCodeAt(i) + ((hash << 5) - hash);
    }
    return hash;
} 

function intToRGB(i){
    var c = (i & 0x00FFFFFF)
        .toString(16)
        .toUpperCase();

    return "#" + "00000".substring(0, 6 - c.length) + c;
}