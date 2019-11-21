using System;
using System.Collections.Generic;

namespace Sudoku.Exceptions
{
    public class CompettingCellIndexesException : Exception
    {
        public CompettingCellIndexesException(List<Tuple<byte, byte>> cells)
        {
            Cells = cells;
        }

        public List<Tuple<byte, byte>> Cells { get; }
    }
}