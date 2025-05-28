using RystBrewery.Software.Database;
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
            new() { Description = "Varm opp til 65°C", Time = 1 },
            new() { Description = "Tilsett malt", Time = 2 },
            new() { Description = "Kok i 60 min", Time = 1 }
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
            new() { Description = "Varm opp til 65°C", Time = 1 },
            new() { Description = "Tilsett malt", Time = 1 },
            new() { Description = "Kok i 60 min", Time = 1 }
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
            new() { Description = "Varm opp til 65°C", Time = 1 },
            new() { Description = "Tilsett malt", Time = 1 },
            new() { Description = "Kok i 60 min", Time = 1 }
        }
                });
            }

            repo.GetAllRecipes();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
