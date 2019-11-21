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

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task UpdateCellValue(byte row, byte col, byte value)
        {
            var connectionId = Context.ConnectionId;
            var user = SudokuCore.GetUserByConnectionId(connectionId);

            try
            {
                SudokuCore.UpdateCell(row, col, value, connectionId);
                
                // обновляем у остальных ползователей ячейку с признаком readonly
                await Clients.AllExcept(connectionId).SendAsync("ReceiveCellValue", row, col, value, user.Name);
            }
            catch (CompettingCellIndexesException exception)
            {
                await Clients.Caller.SendAsync("CompettingCellIndexesException", row, col, exception.Cells);    
            }
        }

        public override async Task OnConnectedAsync()
        {
            // var user = SudokuCore.GetUserByConnectionId(Context.ConnectionId);
            // if (user == null)
            // {
            //     await Clients.Caller.SendAsync("Signup");
            // }
            // else
            // {
            //     await Clients.Caller.SendAsync("DrawField", SudokuCore.CurrentRound.Field);
            // }
        }

        public async Task SignedUser(string name)
        {
            var user = SudokuCore.GetUserByConnectionId(Context.ConnectionId);
            if (user == null)
            {
                user = SudokuCore.CreateUser(Context.ConnectionId, name);
                await Clients.Caller.SendAsync("DrawField", SudokuCore.CurrentRound.Field);
            }
        }
    }
}