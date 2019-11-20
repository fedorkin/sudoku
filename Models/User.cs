namespace Sudoku.Models
{
    public class User
    {
        public string ConnectionId { get; set; }

        public string Name { get; set; }

        public uint NumberOfWins { get; set; }
    }
}