using GamesDiscounts.Models;
using Microsoft.Data.SqlClient;
using System.Windows.Input;
using Telegram.Bot.Types;

namespace GamesDiscounts.Services
{
    public class SqlSaveDB : IDataBase
    {
        private static string connectionString = @"Data Source=MIME;Initial Catalog=SavedGame;Integrated Security=True;Trust Server Certificate=True";
        SqlConnection connection = new SqlConnection(connectionString);

        public async Task SaveGameNameAsync(long ChatId, string GameName)
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            string query = "INSERT INTO SavedGames_db (ChatId, GameName) VALUES (@ChatId, @GameName)";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@ChatId", ChatId);
                command.Parameters.AddWithValue("@GameName", GameName);
                command.ExecuteNonQuery();// Insert the game name into the database
            }
            await conn.CloseAsync();
        }
        public async Task SetAlertsAsync(long ChatId, bool AlertsEnabled, int DiscountPercent)
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            string query = @"
                IF EXISTS (SELECT 1 FROM SavedGames_db WHERE ChatId = @ChatId)
                    UPDATE SavedGames_db
                    SET AlertsEnabled = @AlertsEnabled, DiscountPercent = @DiscountPercent
                    WHERE ChatId = @ChatId
                ELSE
                    INSERT INTO SavedGames_db (ChatId, AlertsEnabled, DiscountPercent)
                    VALUES (@ChatId, @AlertsEnabled, @DiscountPercent);";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@ChatId", ChatId);
                command.Parameters.AddWithValue("@AlertsEnabled", AlertsEnabled);
                command.Parameters.AddWithValue("@DiscountPercent", DiscountPercent);
                command.ExecuteNonQuery();// Insert or update the alert state into the database
            }
            await conn.CloseAsync();
        }
        public async Task<List<AlertSettings>> GetAllEnabledAlertsAsync()
        {
            var alert = new List<AlertSettings>();
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            string query = "SELECT ChatId, AlertsEnabled, DiscountPercent FROM SavedGames_db WHERE AlertsEnabled = 1";
            using (SqlCommand command = new SqlCommand(query, conn))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    alert.Add(new AlertSettings
                    {
                        ChatId = reader.GetInt64(reader.GetOrdinal("ChatId")),
                        IsEnabled = reader.GetBoolean(reader.GetOrdinal("AlertsEnabled")),
                        DiscountPercent = reader.GetInt32(reader.GetOrdinal("DiscountPercent"))

                    });
                } 
            }
            return alert;

        }

        public async Task<List<string>> GetSavedGames(long ChatId)
        {
            List<string> savedGames = new List<string>();
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            string query = "SELECT GameName FROM SavedGames_db WHERE ChatId = @ChatId";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@ChatId", ChatId);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        savedGames.Add(reader["GameName"].ToString());
                    }
                }
            }
            await conn.CloseAsync();
            return savedGames;
        }
        public async Task DeleteGameName(long ChatId, string GameName)
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            string query = "DELETE FROM SavedGames_db WHERE ChatId = @ChatId AND GameName = @GameName";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@ChatId", ChatId);
                command.Parameters.AddWithValue("@GameName", GameName);
                command.ExecuteNonQuery();
            }
            await conn.CloseAsync();
        }
    }
}