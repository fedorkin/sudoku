using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Sudoku.Exceptions;
using Sudoku.Services;

namespace Sudoku.Hubs
{
    public class GameHub : Hub
    {
        public GameHub(ISudokuCore sudokuCore)
        {
            SudokuCore = sudokuCore;
        }

        protected ISudokuCore SudokuCore { get; } 

        public async Task UpdateCellValue(byte row, byte col, byte value)
        {
            var connectionId = Context.ConnectionId;
            var user = SudokuCore.GetUserByConnectionId(connectionId);

            List<Point> competingValueCoordinates = null;
            try
            {
                var cell = SudokuCore.UpdateCell(row, col, value, connectionId);

                // обновляем у остальных пользователей ячейку с признаком readonly
                await Clients.AllExcept(connectionId).SendAsync("ReceiveCellValue", row, col, cell);
            }
            catch (CompetingValueCoordinatesException exception)
            {
                competingValueCoordinates = exception.Cells;
            }

            var userField = SudokuCore.GetFieldForUser(user);
            if (competingValueCoordinates != null)
            {
                SudokuCore.AddCompetingValues(userField, competingValueCoordinates, value);
            }

            await Clients.Caller.SendAsync("DrawField", userField);
        }

        public async Task SignedUser(string name)
        {
            var user = SudokuCore.GetUserByConnectionId(Context.ConnectionId);
            if (user == null)
            {
                user = SudokuCore.CreateUser(Context.ConnectionId, name);
            }

            await Clients.Caller.SendAsync("DrawField", SudokuCore.GetFieldForUser(user));
        }
    }
}