using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
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

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task UpdateCellValue(byte row, byte col, byte value, string userName)
        {
            var connectionId = Context.ConnectionId;
            var user = SudokuCore.GetUserByConnectionId(connectionId);
            if (user == null)
            {
                SudokuCore.CreateUser(connectionId, userName);
            }

            SudokuCore.UpdateCell(row, col, value, connectionId);

            // обновляем у остальных ползователей ячейку с признаком readonly
            await Clients.AllExcept(connectionId).SendAsync("ReceiveCellValue", row, col, value, true);
        }
    }
}