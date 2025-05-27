using RystBrewery.Software.Database;
using System.Configuration;
using System.Data;
using System.Windows;

namespace RystBrewery
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Database.InitializeDatabase();

            var repo = new RecipeRepo();
            repo.AddRecipe(new Recipe
            {
                Name = "IPA",
                Steps = new List<BrewingSteps>
        {
            new() { Description = "Varm opp til 65°C", Time = 1 },
            new() { Description = "Tilsett malt", Time = 2 },
            new() { Description = "Kok i 60 min", Time = 1 }
        }
            });

            var allRecipes = repo.GetAllRecipes();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
