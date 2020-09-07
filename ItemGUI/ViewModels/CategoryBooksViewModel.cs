using ItemApiProject.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemGUI.ViewModels
{
    public class CategoryBooksViewModel
    {
        public CategoryDto Category { get; set; }
        public IEnumerable<BookDto> Books { get; set; }
    }
}
