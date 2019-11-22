using System.Collections.Generic;
using System.Drawing;
using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public interface ISudokuFieldVerifier
    {
        bool Verify(
            Field field,
            int cellRow,
            int cellCol,
            byte cellValue,
            out List<Point> competingValueCoordinates);
    }
}