using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Models;
using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public class SudokuGame : ISudokuGame
    {
        static List<User> users = new List<User>();

        static Round currentRound = null;

        public SudokuGame(ISudokuFieldProvider fieldProvider)
        {
            FieldProvider = fieldProvider;

            if (CurrentRound == null)
            {
                NewRound();
            }
        }

        public Round CurrentRound
        {
            get => currentRound;
            set => currentRound = value;
        }

        public ISudokuFieldProvider FieldProvider { get; }

        public User CreateUser(string connectionId, string name)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (GetUserByConnectionId(connectionId) != null)
            {
                throw new ArgumentException($"User with connectionId = {connectionId} alread exists", 
                    nameof(name));
            }

            var result = new User { ConnectionId = connectionId, Name = name };
            users.Add(result);

            return result;
        }

        public ICollection<User> GetTopUsers(int limit)
        {
            return users.OrderByDescending(u => u.NumberOfWins).ToList();
        }

        public User GetUserByConnectionId(string connectionId)
        {
            return users.SingleOrDefault(u => u.ConnectionId.Equals(connectionId));
        }

        public void NewRound()
        {
            var round = new Round();
            FieldProvider.Fill(round.Field, LevelOfDifficult.Medium);

            CurrentRound = round;
        }

        public Cell UpdateCell(byte rowIndex, byte colIndex, byte value, string connectionId)
        {
            ValidateCellParameters(rowIndex, colIndex, value, connectionId);
            
            var cell = CurrentRound.Field.Cells[rowIndex, colIndex];
            var user = GetUserByConnectionId(connectionId);

            var canWriteValue = string.IsNullOrEmpty(cell.UserConnectionId) || cell.UserConnectionId.Equals(connectionId); 
            if (!canWriteValue)
            {
                throw new AccessViolationException($"The User:{user.Name} has no right to edit the cell[{rowIndex}, {colIndex}]");
            }

            cell.Value = value;
            cell.UserConnectionId = connectionId;

            return cell;
        }

        private void ValidateCellParameters(byte rowIndex, byte colIndex, byte value, string connectionId)
        {
            if (rowIndex >= CurrentRound.Field.Cells.GetLength(0))
            {
                throw new IndexOutOfRangeException($"{nameof(rowIndex)}:{rowIndex} can't be more or equal field rank");
            }

            if (colIndex >= CurrentRound.Field.Cells.GetLength(1))
            {
                throw new IndexOutOfRangeException($"{nameof(colIndex)}:{colIndex} can't be more or equal field rank");
            }

            if (value > 9)
            {
                throw new ArgumentException($"{nameof(value)}:{value} can't be greater than 9");
            }

            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }
        }
    }
}