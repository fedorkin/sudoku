namespace Sudoku.Models.Game
{
    public struct Cell
    {
        public byte Value { get; set; }

        public bool ReadOnly { get; set; }

        public string UserConnectionId { get; set; }
    }
}