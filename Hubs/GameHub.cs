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
            var player = SudokuCore.GetPlayerByConnectionId(connectionId);

            try
            {
                // выставим значение в общем поле
                var cell = SudokuCore.UpdateCell(row, col, value, connectionId);

                // выставляем данное значение другим пользователем
                await Clients.AllExcept(connectionId).SendAsync("ReceiveCellValue", row, col, cell);
                // отрисуем участников
                await Clients.All.SendAsync("ReceivePlayers", SudokuCore.GetTopPlayers(10));

                // если пользователь выставляет 0, скорей всего он отменяет неправильный ход
                // нам следует перерисовать его поле целиком, чтобы убрать предыдущие ошибки
                if (value == 0)
                {
                    await Clients.Caller.SendAsync("DrawField", SudokuCore.GetFieldForPlayer(player));
                }

                if (SudokuCore.IsEndGame())
                {
                    await NewGame();
                    return;
                }
            }
            catch (CompetingValueCoordinatesException exception)
            {
                var playerField = SudokuCore.GetFieldForPlayer(player);
                // пометим конфликтующие ячейки
                SudokuCore.AddCompetingValues(playerField, exception.Cells, value);
                // покажем пользователю ошибки
                await Clients.Caller.SendAsync("DrawField", playerField);
            }
        }

        public async Task SignedPlayer(string name)
        {
            var player = SudokuCore.GetPlayerByConnectionId(Context.ConnectionId);
            if (player == null)
            {
                player = SudokuCore.CreatePlayer(Context.ConnectionId, name);
            }

            await Clients.Caller.SendAsync("DrawField", SudokuCore.GetFieldForPlayer(player));
            await Clients.All.SendAsync("ReceivePlayers", SudokuCore.GetTopPlayers(10));
        }

        public async Task NewGame()
        {
            // создадим новое поле и отрисуем его у всех игроков
            SudokuCore.NewRound();
            await Clients.All.SendAsync("ReceivePlayers", SudokuCore.GetTopPlayers(10));
            await Clients.All.SendAsync("DrawField", SudokuCore.CurrentRound.Field);
        }
    }
}