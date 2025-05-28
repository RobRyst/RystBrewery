using RystBrewery.Software.Database;
using RystBrewery.Software.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;

namespace RystBrewery
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Database.InitializeDatabase();

            var recipeRepo = new RecipeRepo();
            if (!recipeRepo.RecipeExists("Ryst IPA"))
            {
                recipeRepo.AddRecipe(new Recipe
                {
                    Name = "Ryst IPA",
                    Steps = new List<BrewingSteps>
        {
            new() { Description = "Varm opp til 65°C", Time = 5 },
            new() { Description = "Tilsett malt", Time = 3 },
            new() { Description = "Kok i 60 min", Time = 10 }
        }
                });
            }

            if (!recipeRepo.RecipeExists("Ryst Sommerøl"))
            {
                recipeRepo.AddRecipe(new Recipe
                {
                    Name = "Ryst Sommerøl",
                    Steps = new List<BrewingSteps>
        {
            new() { Description = "Varm opp til 65°C", Time = 5 },
            new() { Description = "Tilsett Sommer", Time = 3 },
            new() { Description = "Kok i 60 min", Time = 10 }
        }
                });
            }

            if (!recipeRepo.RecipeExists("Ryst Eplecider"))
            {
                recipeRepo.AddRecipe(new Recipe
                {
                    Name = "Ryst Eplecider",
                    Steps = new List<BrewingSteps>
        {
            new() { Description = "Varm opp til 65°C", Time = 5 },
            new() { Description = "Tilsett EPLE", Time = 3 },
            new() { Description = "Kok i 60 min", Time = 10 }
        }
                });
            }

            var washingRepo = new WashingRepo();
            if (!washingRepo.WashProgramExists("Ryst IPA Wash"))
            {
                washingRepo.AddWashProgram(new WashProgram
                {
                    Name = "Ryst IPA Washing Program",
                    Steps = new List<WashingSteps>
        {
            new() { Description = "Tømmer Tank", Time = 5 },
            new() { Description = "Blander Vaskemiddel", Time = 3 },
            new() { Description = "Spyl i 30min", Time = 10 }
        }
                });
            }

            recipeRepo.GetAllRecipes();
            washingRepo.getAllWashPrograms();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
