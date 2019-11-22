using System.Collections.Generic;
using System.Drawing;
using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public class SudokuFieldVerifier : ISudokuFieldVerifier
    {
        public bool Verify(
            Field field,
            int cellRow,
            int cellCol,
            byte cellValue,
            out List<Point> competingValueCoordinates)
        {
            var result = true;

            // координаты повтороящихся значений
            competingValueCoordinates = new List<Point>();
            result &= RowVerify(field, cellRow, cellCol, cellValue, competingValueCoordinates);
            result &= ColVerify(field, cellRow, cellCol, cellValue, competingValueCoordinates);
            result &= SubFieldVerify(field, cellRow, cellCol, cellValue, competingValueCoordinates);

            if (!result)
            {
                competingValueCoordinates.Add(new Point(cellRow, cellCol));
            }

            return result;
        }

        private bool RowVerify(
            Field field,
            int cellRow,
            int cellCol,
            byte cellValue,
            List<Point> competingValueCoordinates)
        {
            var result = true;

            for (var col = 0; col < field.Rank; col++)
            {
                var fieldCellValue = field.Cells[cellRow, col].Value; 
                if ((fieldCellValue > 0) && fieldCellValue.Equals(cellValue))
                {
                    competingValueCoordinates.Add(new Point(cellRow, col));
                    result = false;
                }
            }

            return result;
        }

        private bool ColVerify(
            Field field,
            int cellRow,
            int cellCol,
            byte cellValue,
            List<Point> competingValueCoordinates)
        {
            var result = true;

            for (var row = 0; row < field.Rank; row++)
            {
                var fieldCellValue = field.Cells[row, cellCol].Value;
                if ((fieldCellValue > 0) && fieldCellValue.Equals(cellValue))
                {
                    competingValueCoordinates.Add(new Point(row, cellCol));
                    result = false;
                }
            }

            return result;
        }

        private bool SubFieldVerify(
            Field field,
            int cellRow,
            int cellCol,
            byte cellValue,
            List<Point> competingValueCoordinates)
        {
            var result = true;

            var startSubRow = (cellRow / 3) * 3;
            var starutSubCol = (cellCol / 3) * 3;
            
            for (var subRow = startSubRow; subRow < startSubRow + 3; subRow++)
            {
                for (var subCol = starutSubCol; subCol < starutSubCol + 3; subCol++)
                {
                    var fieldCellValue = field.Cells[subRow, subCol].Value; 
                    if ((fieldCellValue > 0) && fieldCellValue.Equals(cellValue))
                    {
                        competingValueCoordinates.Add(new Point(subRow, subCol));
                        result = false;
                    }
                }
            }

            return result;
        }
    }
}