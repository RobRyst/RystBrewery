using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RystBrewery.Software.Database
{
    public class WashProgram
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WashingSteps> Steps { get; set; } = new List<WashingSteps>();
    }

    public class WashingSteps
    {
        public string Description { get; set; }

        public int Time { get; set; }
    }

        public class WashingRepo
    {

            private const string DbFileName = "RystBrewery.db";

            public void AddWashProgram(WashProgram washProgram)
            {

                using var connection = new SqliteConnection($"Data Source={DbFileName}");
                connection.Open();

                var insertWashProgram = connection.CreateCommand();
                insertWashProgram.CommandText = "INSERT INTO WashProgram (Name) VALUES ($name); SELECT last_insert_rowid();";
                insertWashProgram.Parameters.AddWithValue("$name", washProgram.Name);
                var washingId = (long)insertWashProgram.ExecuteScalar();

                foreach (var step in washProgram.Steps)
                {
                    var insertStep = connection.CreateCommand();
                    insertStep.CommandText = @"
                        INSERT INTO WashingSteps (WashingId, Description, Time)
                        VALUES ($washingId, $description, $time);";
                    insertStep.Parameters.AddWithValue("$washingId", washingId);
                    insertStep.Parameters.AddWithValue("$description", step.Description);
                    insertStep.Parameters.AddWithValue("$time", step.Time);
                    insertStep.ExecuteNonQuery();
                }
            }

            public bool WashProgramExists(string name)
            {
                using var connection = new SqliteConnection($"Data Source = {DbFileName}");
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = ("SELECT COUNT(*) FROM WashProgram WHERE Name = $name");
                cmd.Parameters.AddWithValue("$name", name);

                var count = (long)cmd.ExecuteScalar();
                return count > 0;
            }

            public List<WashProgram> GetAllWashPrograms()
            {
                var washPrograms = new List<WashProgram>();

                using var connection = new SqliteConnection($"Data Source = {DbFileName}");
                connection.Open();

                var washCmd = connection.CreateCommand();
                washCmd.CommandText = "SELECT Id, Name FROM WashProgram";
                using var reader = washCmd.ExecuteReader();
                while (reader.Read())
                {
                    var washProgram = new WashProgram()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)

                    };
                    washPrograms.Add(washProgram);
                }
                foreach (var washprogram in washPrograms)
                {
                    var stepCmd = connection.CreateCommand();
                    stepCmd.CommandText = "SELECT Description, Time FROM WashingSteps WHERE WashingId = $id";
                    stepCmd.Parameters.AddWithValue("$id", washprogram.Id);
                    using var stepReader = stepCmd.ExecuteReader();
                    while (stepReader.Read())
                    {
                        washprogram.Steps.Add(new WashingSteps
                        {
                            Description = stepReader.GetString(0),
                            Time = stepReader.GetInt32(1)
                        });
                    }
                }
                return washPrograms;
            }
        }
    }