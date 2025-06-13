using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RystBrewery.Software.Models
{
    public class WashProgram
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<WashingSteps> WashingSteps { get; set; } = new List<WashingSteps>();
    }

    public class WashingSteps
    {
        public required string Description { get; set; }
        public int Time { get; set; }
    }
}
