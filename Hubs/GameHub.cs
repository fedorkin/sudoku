using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Sudoku.Models.Game;
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

        public async Task UpdateCell(byte row, byte col, byte value)
        {
            var connectionId = Clients.
        }
    }
}