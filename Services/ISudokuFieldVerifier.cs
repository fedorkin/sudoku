using System;
using System.Collections.Generic;
using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public interface ISudokuFieldVerifier
    {
        bool Verify(Field field, byte cellRow, byte cellCol, byte cellValue, out List<Tuple<byte, byte>> competingCellIndexes);
    }
}