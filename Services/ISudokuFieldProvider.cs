using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public enum LevelOfDifficult
    {
        Simple = 1,

        Medium = 2,

        Hard = 3
    }

    public interface ISudokuFieldProvider
    {
         void Fill(Field field, LevelOfDifficult level);
    }
}