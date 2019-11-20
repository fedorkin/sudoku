using System.Collections.Generic;
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

        Cell UpdateCell(byte rowIndex, byte colIndex, byte value, string connectionId);
    }
}