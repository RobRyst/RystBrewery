using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

namespace RystBrewery.Software.Database
{
    public class PasswordAuth
    {
        private const string DbFileName = "RystBrewery.db";

        public bool AuthPassword(string passwordInput)
        {
            using var connection = new SqliteConnection($"Data Source={DbFileName}");
            connection.Open();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT PasswordHash FROM PasswordAuth WHERE Id = 1";
            var result = selectCmd.ExecuteScalar();

            if (result == null) 
                return false;

            string storedHash = (string)result;
            string hashInput = HashPassword(passwordInput);
            return storedHash == hashInput;
        }

        private string HashPassword(string passwordAuthKey)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordAuthKey));
            return Convert.ToBase64String(bytes);
        }
    }
}