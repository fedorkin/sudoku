namespace Sudoku.Models.Game
{
    public class Field
    {
        public Cell[,] Cells { get; }

        public byte Rank { get; }

        public Field (byte rank)
        {
            Rank = rank;
            Cells = new Cell[Rank, Rank];
        }
    }
}