using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Microsoft.Data.Sqlite;
using RystBrewery.Software.Database;

namespace RystBrewery.Software.Database
{

    public class BrewingSteps
    {
        public string Description { get; set; }
        public int Time { get; set; }
    }

    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BrewingSteps> Steps { get; set; } = new List<BrewingSteps>();
    }

    public class RecipeRepo
    {
        private const string DbFileName = "RystBrewery.db";

        public void AddRecipe(Recipe recipe)
        {

            using var connection = new SqliteConnection($"Data Source={DbFileName}");
            connection.Open();

            var insertRecipe = connection.CreateCommand();
            insertRecipe.CommandText = "INSERT INTO Recipes (Name) VALUES ($name); SELECT last_insert_rowid();";
            insertRecipe.Parameters.AddWithValue("$name", recipe.Name);
            var recipeId = (long)insertRecipe.ExecuteScalar();

            foreach (var step in recipe.Steps)
            {
                var insertStep = connection.CreateCommand();
                insertStep.CommandText = @"
                    INSERT INTO BrewingSteps (RecipeId, Description, Time)
                    VALUES ($recipeId, $description, $time);";
                insertStep.Parameters.AddWithValue("$recipeId", recipeId);
                insertStep.Parameters.AddWithValue("$description", step.Description);
                insertStep.Parameters.AddWithValue("$time", step.Time);
                insertStep.ExecuteNonQuery();
            }
        }

        public bool RecipeExists(string name)
        {
            using var connection = new SqliteConnection($"Data Source={DbFileName}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Recipes WHERE Name = $name";
            cmd.Parameters.AddWithValue("$name", name);

            var count = (long)cmd.ExecuteScalar();
            return count > 0;
        }

        public Recipe? GetRecipeByName(string name)
        {
            return GetAllRecipes().FirstOrDefault(r => r.Name == name);
        }

        public List<Recipe> GetAllRecipes()
        {
            var recipes = new List<Recipe>();

            using var connection = new SqliteConnection($"Data Source={DbFileName}");
            connection.Open();

            var recipeCmd = connection.CreateCommand();
            recipeCmd.CommandText = "SELECT Id, Name FROM Recipes";
            using var reader = recipeCmd.ExecuteReader();
            while (reader.Read())
            {
                var recipe = new Recipe
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
                recipes.Add(recipe);
            }
            foreach (var recipe in recipes)
            {
                var stepCmd = connection.CreateCommand();
                stepCmd.CommandText = "SELECT Description, Time FROM BrewingSteps WHERE RecipeId = $id";
                stepCmd.Parameters.AddWithValue("$id", recipe.Id);
                using var stepReader = stepCmd.ExecuteReader();
                while (stepReader.Read())
                {
                    recipe.Steps.Add(new BrewingSteps
                    {
                        Description = stepReader.GetString(0),
                        Time = stepReader.GetInt32(1)

                    });
                }
            }
            return recipes;
        }
    }
}

