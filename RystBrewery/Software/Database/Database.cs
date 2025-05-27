using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RystBrewery.Software.Database
{
    public static class Database
    {
        private const string DbFileName = "recipes.db";

        public static void InitializeDatabase()
        {
            if (!File.Exists(DbFileName))
                {
                using var connection = new SqliteConnection($"Data Source={DbFileName}");
                connection.Open();

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @"
                CREATE TABLE Recipes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name INTEGER NOT NULL
                    );

                CREATE TABLE BrewingSteps (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RecipeId INTEGER NOT NULL,
                    Description TEXT NOT NULL,
                    Time INTEGER NOT NULL,
                    FOREIGN KEY (RecipeId) REFERENCES Recipes(Id)
                );";
                tableCmd.ExecuteNonQuery();
            }
        }
    }
}
