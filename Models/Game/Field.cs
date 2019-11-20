namespace Sudoku.Models.Game
{
    public class Field
    {
        public Cell[,] Cells { get; }

        public Field (byte rank)
        {
            Cells = new Cell[rank, rank];
        }
    }
}