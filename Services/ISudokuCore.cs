using System.Collections.Generic;
using System.Drawing;
using Sudoku.Models;
using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public interface ISudokuCore
    {
        Round CurrentRound { get; }

        void NewRound();

        Player CreatePlayer(string connectionId, string name);

        Player GetPlayerByConnectionId(string connectionId);

        ICollection<Player> GetTopPlayers(int limit);

        Cell UpdateCell(int rowIndex, int colIndex, byte value, string connectionId);

        Field GetFieldForPlayer(Player player);

        void AddCompetingValues(Field field, List<Point> competingValueCoordinates, byte value);

        bool IsEndGame();
    }
}