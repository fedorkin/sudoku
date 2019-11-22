using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sudoku.Exceptions
{
    public class CompetingValueCoordinatesException : Exception
    {
        public CompetingValueCoordinatesException(List<Point> cells)
        {
            Cells = cells;
        }

        public List<Point> Cells { get; }
    }
}