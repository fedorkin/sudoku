namespace Sudoku.Models.Game
{
    public struct Cell
    {
        public byte Value { get; set; }
        
        public bool Readonly { get; set; }

        public string UserConnectionId { get; set; }
    }
}