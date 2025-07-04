﻿namespace RystBrewery.Software.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<BrewingSteps> BrewingSteps { get; set; } = new List<BrewingSteps>();
    }

    public class  BrewingSteps
    {
        public required string Description { get; set; }
        public required string Time { get; set; }
    }
}
