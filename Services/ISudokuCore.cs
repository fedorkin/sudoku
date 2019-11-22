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

        User CreateUser(string connectionId, string name);

        User GetUserByConnectionId(string connectionId);

        ICollection<User> GetTopUsers(int limit);

        Cell UpdateCell(int rowIndex, int colIndex, byte value, string connectionId);

        Field GetFieldForUser(User user);

        void AddCompetingValues(Field field, List<Point> competingValueCoordinates, byte value);
    }
}