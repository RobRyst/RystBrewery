using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RystBrewery.Software.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BrewingSteps> Steps { get; set; }
    }

    public class  BrewingSteps
    {
        public string Description { get; set; }
        public string Time { get; set; }
    }
}
