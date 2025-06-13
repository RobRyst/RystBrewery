using System.Text;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

namespace RystBrewery.Software.Database
{
    public static class PasswordSettings
    {
        private const string DbFileName = "RystBrewery.db";

        public static void Password()
        {
            const string passwordAuthKey = "12345";
            string hash = HashPassword(passwordAuthKey);

            using var connection = new SqliteConnection($"Data Source={DbFileName}");
            connection.Open();

            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS PasswordAuth (
                    Id INTEGER PRIMARY KEY, 
                    PasswordHash TEXT NOT NULL
                );";

            createTableCmd.ExecuteNonQuery();

            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
                INSERT OR REPLACE INTO PasswordAuth (Id, PasswordHash)
                VALUES (1, @hash);";

            insertCmd.Parameters.AddWithValue("@hash", hash);
            insertCmd.ExecuteNonQuery();

            Console.WriteLine("Password initialized successfully");
        }

        private static string HashPassword(string passwordAuthKey)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordAuthKey));
            return Convert.ToBase64String(bytes);
        }
    }
}