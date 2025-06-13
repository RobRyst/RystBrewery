using RystBrewery.Software.Database;
using System.Windows;

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
                        new() { Description = "Varm opp til 60°C", Time = 10 },
                        new() { Description = "Tilsett Malt og Humle", Time = 5 },
                        new() { Description = "Kok i 15 min", Time = 15 }
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
                        new() { Description = "Varm opp til 60°C", Time = 10 },
                        new() { Description = "Tilsett Malt og Einer", Time = 5 },
                        new() { Description = "Kok i 15 min", Time = 15 }
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
                        new() { Description = "Varm opp til 60°C", Time = 10 },
                        new() { Description = "Tilsett Eple Juice", Time = 5 },
                        new() { Description = "Kok i 15 min", Time = 15 }
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
                        new() { Description = "Tømmer og renser tank", Time = 10 },
                        new() { Description = "Vaskemiddel tilsettes", Time = 5 },
                        new() { Description = "Renser tank for vaskemiddel", Time = 15 }
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
                        new() { Description = "Tømmer og renser tank", Time = 10 },
                        new() { Description = "Vaskemiddel tilsettes", Time = 5 },
                        new() { Description = "Renser tank for vaskemiddel", Time = 15 }
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
                        new() { Description = "Tømmer og renser tank", Time = 10 },
                        new() { Description = "Vaskemiddel tilsettes", Time = 5 },
                        new() { Description = "Renser tank for vaskemiddel", Time = 15 }
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