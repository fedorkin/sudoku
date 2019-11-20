using System.Collections.Generic;
using System.Threading.Tasks;
using Sudoku.Models;
using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public interface ISudokuGame
    {
        Round CurrentRound { get; }

        Round NewRound();

        User CreateUser(string connectionId, string name);

        User GetUserByConnectionId(string connectionId);

        ICollection<User> GetTopUsers(int limit);

        Cell UpdateCell(byte rowIndex, byte colIndex, byte value, string connectionId);
    }
}