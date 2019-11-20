using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sudoku.Services;

namespace Sudoku.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;


        public IndexModel(ILogger<IndexModel> logger, ISudokuCore sudokuGame)
        {
            _logger = logger;
            SudokuGame = sudokuGame;
        }
        
        public ISudokuCore SudokuGame { get; }

        public void OnGet()
        {
        }
    }
}