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

            
            var repo = new RecipeRepo();
            if (!repo.RecipeExists("Ryst IPA"))
            {
                repo.AddRecipe(new Recipe
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

            if (!repo.RecipeExists("Ryst Sommerøl"))
            {
                repo.AddRecipe(new Recipe
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

            if (!repo.RecipeExists("Ryst Eplecider"))
            {
                repo.AddRecipe(new Recipe
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

            repo.GetAllRecipes();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
