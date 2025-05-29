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
        public string Name { get; set; }
        public List<WashingSteps> WashingSteps { get; set; }
    }

    public class WashingSteps
    {
        public string Description { get; set; }
        public int Time { get; set; }
    }
}
