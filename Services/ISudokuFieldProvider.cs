using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public enum LevelOfDifficult
    {
        Simple = 0,

        Medium = 1,

        Hard = 2
    }

    public interface ISudokuFieldProvider
    {
         void Fill(Field field, LevelOfDifficult level);
    }
}