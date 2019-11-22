using System;
using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public class SudokuFieldProvider : ISudokuFieldProvider
    {
        public void Fill(Field field, LevelOfDifficult level)
        {
            Init(field);
            MixValues(field, level);
            CutValues(field, level);
        }

        private void Init(Field field)
        {
            for(byte row = 0; row < field.Rank; row++)
            {
                for (byte col = 0; col < field.Rank; col++)
                {
                    field.Cells[row, col].Value = (byte)(((row * 3) + (row / 3) + col) % field.Rank + 1);
                }
            }
        }

        private void ChangeCellValue(Field field, byte firstTargetValue, byte secondTargetValue)
        {
            for (var row = 0; row < field.Rank; row += 3)
            {
                for (var col = 0; col < field.Rank; col += 3)
                {
                    for (var subRow = 0; subRow < 3; subRow++)
                    {
                        for (var subCol = 0; subCol < 3; subCol++)
                        {
                            ref Cell currentCell = ref field.Cells[row + subRow, col + subCol];

                            if (currentCell.Value.Equals(firstTargetValue))
                            {
                                currentCell.Value = secondTargetValue;
                                continue;
                            }

                            if (currentCell.Value.Equals(secondTargetValue))
                            {
                                currentCell.Value = firstTargetValue;
                            }
                        }
                    }
                }
            }
        }

        private void MixValues(Field field, LevelOfDifficult level)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            var interationNumber = (int)level * 3; 

            for (var i = 0; i < interationNumber; i++)
            {
                ChangeCellValue(field, (byte)random.Next(1, field.Rank), (byte)random.Next(1, field.Rank));
            }
        }

        private void CutValues(Field field, LevelOfDifficult level)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            var cutValuesNumber = (int)level * 20;

            for (var i = 0; i < cutValuesNumber; i++)
            {
                var row = random.Next(0, field.Rank);
                var col = random.Next(0, field.Rank);

                field.Cells[row, col].Value = 0;
            }
        }
    }
}