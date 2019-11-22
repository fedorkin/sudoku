using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sudoku.Exceptions;
using Sudoku.Models;
using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public class SudokuCore : ISudokuCore
    {
        static List<User> users = new List<User>();

        static Round currentRound = null;

        public SudokuCore(ISudokuFieldProvider fieldProvider, ISudokuFieldVerifier fieldVerifier)
        {
            FieldProvider = fieldProvider;
            FieldVerifier = fieldVerifier;

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

        public ISudokuFieldVerifier FieldVerifier { get; }

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

        public Field GetFieldForUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var commonField = CurrentRound.Field;
            var userField = new Field(commonField.Rank);

            for (var row = 0; row < commonField.Cells.GetLength(0); row++)
            {
                for (var col = 0; col < commonField.Cells.GetLength(1); col++)
                {
                    var userCell = commonField.Cells[row, col];
                    userCell.Editable = (userCell.Value == 0) || user.Name.Equals(userCell.UserName);
                    userField.Cells[row, col] = userCell;
                }
            }

            return userField;
        }

        public void AddCompetingValues(Field field, List<Point> competingValueCoordinates, byte value)
        {
            foreach(var cooordinate in competingValueCoordinates)
            {
                ref Cell cell = ref field.Cells[cooordinate.X, cooordinate.Y];
                cell.IsCompeting = true;
                cell.Value = value;
            }
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

        public Cell UpdateCell(int rowIndex, int colIndex, byte value, string connectionId)
        {
            ValidateCellParameters(rowIndex, colIndex, value, connectionId);
            
            ref Cell cell = ref CurrentRound.Field.Cells[rowIndex, colIndex];
            var user = GetUserByConnectionId(connectionId);

            var canWriteValue = string.IsNullOrEmpty(cell.UserName) || cell.UserName.Equals(user.Name); 
            if (!canWriteValue)
            {
                throw new AccessViolationException($"The User:{user.Name} has no right to edit the cell[{rowIndex}, {colIndex}]");
            }

            if (!FieldVerifier.Verify(CurrentRound.Field, rowIndex, colIndex, value, out List<Point> compettingCellIndexes))
            {
                throw new CompetingValueCoordinatesException(compettingCellIndexes);
            }

            cell.Value = value;
            cell.UserName = value.Equals(0) ? null : user.Name;
            cell.Editable = value.Equals(0);

            return cell;
        }
        
        private void ValidateCellParameters(int rowIndex, int colIndex, byte value, string connectionId)
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