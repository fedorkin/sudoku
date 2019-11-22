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
        static List<Player> players = new List<Player>();

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

        public Player CreatePlayer(string connectionId, string name)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (GetPlayerByConnectionId(connectionId) != null)
            {
                throw new ArgumentException($"Player with connectionId = {connectionId} alread exists", 
                    nameof(name));
            }

            var result = new Player { ConnectionId = connectionId, Name = name };
            players.Add(result);

            return result;
        }

        public Field GetFieldForPlayer(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            var commonField = CurrentRound.Field;
            var playerField = new Field(commonField.Rank);

            for (var row = 0; row < commonField.Cells.GetLength(0); row++)
            {
                for (var col = 0; col < commonField.Cells.GetLength(1); col++)
                {
                    var playerCell = commonField.Cells[row, col];
                    playerCell.Editable = (playerCell.Value == 0) || player.Name.Equals(playerCell.PlayerName);
                    playerField.Cells[row, col] = playerCell;
                }
            }

            return playerField;
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

        public ICollection<Player> GetTopPlayers(int limit)
        {
            return players.OrderByDescending(u => u.Scores).ToList();
        }

        public Player GetPlayerByConnectionId(string connectionId)
        {
            return players.SingleOrDefault(u => u.ConnectionId.Equals(connectionId));
        }

        public void NewRound()
        {
            var round = new Round();
            FieldProvider.Fill(round.Field, LevelOfDifficult.Simple);

            CurrentRound = round;
        }

        public Cell UpdateCell(int rowIndex, int colIndex, byte value, string connectionId)
        {
            ValidateCellParameters(rowIndex, colIndex, value, connectionId);
            
            ref Cell cell = ref CurrentRound.Field.Cells[rowIndex, colIndex];
            var player = GetPlayerByConnectionId(connectionId);

            var canWriteValue = string.IsNullOrEmpty(cell.PlayerName) || cell.PlayerName.Equals(player.Name); 
            if (!canWriteValue)
            {
                throw new AccessViolationException($"The Player:{player.Name} has no right to edit the cell[{rowIndex}, {colIndex}]");
            }

            if (!FieldVerifier.Verify(CurrentRound.Field, rowIndex, colIndex, value, out List<Point> compettingCellIndexes))
            {
                throw new CompetingValueCoordinatesException(compettingCellIndexes);
            }

            cell.Value = value;
            cell.PlayerName = value.Equals(0) ? null : player.Name;
            cell.Editable = value.Equals(0);
            
            CalculateScores();

            return cell;
        }

        public bool IsEndGame()
        {
            foreach(var cell in CurrentRound.Field.Cells)
            {
                if (cell.Value == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void CalculateScores()
        {
            players.ForEach(u => u.Scores = 0);

            foreach(var cell in CurrentRound.Field.Cells)
            {
                var player = players.FirstOrDefault(u => u.Name.Equals(cell.PlayerName));

                if (player != null)
                {
                    player.Scores++;
                }
            }
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