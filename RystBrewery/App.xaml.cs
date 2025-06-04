using RystBrewery.Software.Database;
using RystBrewery.Software.ViewModels;
using RystBrewery.Software.Services;
using System.Configuration;
using System.Data;
using System.Windows;
using RystBrewery.Software;

namespace RystBrewery
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppService.Init();
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
                        new() { Description = "Varm opp til 65°C", Time = 10 },
                        new() { Description = "Tilsett malt", Time = 5 },
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
                        new() { Description = "Varm opp til 65°C", Time = 10 },
                        new() { Description = "Tilsett Sommer", Time = 5 },
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
                        new() { Description = "Varm opp til 65°C", Time = 10 },
                        new() { Description = "Tilsett EPLE", Time = 5 },
                        new() { Description = "Kok i 60 min", Time = 10 }
                    }
                });
            }

            var washingRepo = new WashingRepo();

            if (!washingRepo.WashProgramExists("Ryst IPA Washing"))
            {
                washingRepo.AddWashProgram(new WashProgram
                {
                    Name = "Ryst IPA Washing",
                    Steps = new List<WashingSteps>
                    {
                        new() { Description = "Tømmer Tank", Time = 10 },
                        new() { Description = "Blander Vaskemiddel", Time = 5 },
                        new() { Description = "Spyl i 30min", Time = 10 }
                    }
                });
            }

            if (!washingRepo.WashProgramExists("Ryst Eplecider Washing"))
            {
                washingRepo.AddWashProgram(new WashProgram
                {
                    Name = "Ryst Eplecider Washing",
                    Steps = new List<WashingSteps>
                    {
                        new() { Description = "Tømmer Epler", Time = 10 },
                        new() { Description = "Blander Vaskemiddel", Time = 5 },
                        new() { Description = "Spyl i 30min", Time = 10 }
                    }
                });
            }

            if (!washingRepo.WashProgramExists("Ryst Sommerøl Washing"))
            {
                washingRepo.AddWashProgram(new WashProgram
                {
                    Name = "Ryst Sommerøl Washing",
                    Steps = new List<WashingSteps>
                    {
                        new() { Description = "Tømmer Sommerøl rester", Time = 10 },
                        new() { Description = "Blander Vaskemiddel", Time = 5 },
                        new() { Description = "Spyl i 30min", Time = 10 }
                    }
                });
            }

            recipeRepo.GetAllRecipes();
            washingRepo.GetAllWashPrograms();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}