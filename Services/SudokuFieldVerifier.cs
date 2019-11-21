using System;
using System.Collections.Generic;
using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public class SudokuFieldVerifier : ISudokuFieldVerifier
    {
        public bool Verify(Field field, byte cellRow, byte cellCol, byte cellValue, out List<Tuple<byte, byte>> competingCellIndexes)
        {
            var result = true;
            competingCellIndexes = new List<Tuple<byte, byte>>();
            result &= RowVerify(field, cellRow, cellCol, cellValue, competingCellIndexes);
            result &= ColVerify(field, cellRow, cellCol, cellValue, competingCellIndexes);

            return result;

            // for (int row = 0; row < field.Rank; row += 3)
            // {
            //     for (int col = 0; col < field.Rank; col += 3)
            //     {
            //         for (int subRow = 0; subRow < 3; subRow++)
            //         {
            //             for (int subCol = 0; subCol < 3; subCol++)
            //             {
            //                 ref Cell currentCell = ref field.Cells[row + subRow, col + subCol];

            //                 if (currentCell.Value.Equals(firstTargetValue))
            //                 {
            //                     currentCell.Value = secondTargetValue;
            //                     continue;
            //                 }

            //                 if (currentCell.Value.Equals(secondTargetValue))
            //                 {
            //                     currentCell.Value = firstTargetValue;
            //                 }
            //             }
            //         }
            //     }
            // }
        }

        private bool RowVerify(Field field, byte cellRow, byte cellCol, byte cellValue, List<Tuple<byte, byte>> competingCellIndexes)
        {
            var result = true;

            for (byte col = 0; col < field.Rank; col++)
            {
                var fieldCellValue = field.Cells[cellRow, col].Value; 
                if ((fieldCellValue > 0) && fieldCellValue.Equals(cellValue))
                {
                    competingCellIndexes.Add(new Tuple<byte, byte>(cellRow, col));
                    result = false;
                }
            }

            return result;
        }

        private bool ColVerify(Field field, byte cellRow, byte cellCol, byte cellValue, List<Tuple<byte, byte>> competingCellIndexes)
        {
            var result = true;

            for (byte row = 0; row < field.Rank; row++)
            {
                var fieldCellValue = field.Cells[row, cellCol].Value; 
                if ((fieldCellValue > 0) && fieldCellValue.Equals(cellValue))
                {
                    competingCellIndexes.Add(new Tuple<byte, byte>(row, cellCol));
                    result = false;
                }
            }

            return result;
        }
    }
}