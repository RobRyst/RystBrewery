using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace RystBrewery.Software.Database
{
    public static class Database
    {
        private const string DbFileName = "RystBrewery.db";

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
                    Name TEXT NOT NULL
                    );

                CREATE TABLE BrewingSteps (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RecipeId INTEGER NOT NULL,
                    Description TEXT NOT NULL,
                    Time INTEGER NOT NULL,
                    FOREIGN KEY (RecipeId) REFERENCES Recipes(Id)
                );


                CREATE TABLE WashProgram (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT NOT NULL
                );

                CREATE TABLE WashingSteps (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    WashingId INTEGER NOT NULL,
                    Description TEXT NOT NULL,
                    Time INTEGER NOT NULL,
                    FOREIGN KEY (WashingId) REFERENCES WashProgram(Id)
            );

                CREATE TABLE AccessPassword {
                    Id INTEGER PRIMARY KEY, 
                    Password TEXT NOT NULL, 
            }";

                tableCmd.ExecuteNonQuery();
            }
        }
    }
}
