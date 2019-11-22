namespace Sudoku.Models.Game
{
    public struct Cell
    {
        public byte Value { get; set; }
        
        public bool Editable { get; set; }

        public bool IsCompeting { get; set; }

        public string PlayerName { get; set; }
    }
}