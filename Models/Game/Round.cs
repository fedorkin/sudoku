namespace Sudoku.Models.Game
{
    public class Round
    {
        public Field Field { get; }

        public Round()
        {
            Field = new Field(9);
        }
    }
}