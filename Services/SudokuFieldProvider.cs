using Sudoku.Models.Game;

namespace Sudoku.Services
{
    public class SudokuFieldProvider : ISudokuFieldProvider
    {
        public void Fill(Field field, LevelOfDifficult level)
        {
            Init(field);
        }

        private void Init(Field field)
        {
            for(byte i = 0; i < field.Rank; i++)
            {
                for (byte j = 0; j < field.Rank; j++)
                {
                    field.Cells[i, j].Value = (byte)((i * 3 + i / 3 + j) % 9 + 1);
                }
            }
        }
    }
}