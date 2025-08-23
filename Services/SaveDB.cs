using GamesDiscounts.Models;
using Microsoft.Data.SqlClient;

namespace GamesDiscounts.Services
{
    public class SaveDB: IDataBase
    {
        SqlConnection connection = new SqlConnection(@"Data Source=MIME;Initial Catalog=SavedGame;Integrated Security=True;Trust Server Certificate=True");

        public void SaveGameName(long ChatId, string GameName)
        {
            connection.Open();
            string query = "INSERT INTO SavedGames_db (ChatId, GameName) VALUES (@ChatId, @GameName)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ChatId", ChatId);

                command.Parameters.AddWithValue("@GameName", GameName);
                command.ExecuteNonQuery();// Insert the game name into the database
            }
            connection.Close();
        }
        public List<string> GetSavedGames(long ChatId)
        {
            List<string> savedGames = new List<string>();
            connection.Open();
            string query = "SELECT GameName FROM SavedGames_db WHERE ChatId = @ChatId";
            using (SqlCommand command = new SqlCommand(query, connection))
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
            connection.Close();
            return savedGames;
        }
        public void DeleteGameName(long ChatId, string GameName)
        {
            connection.Open();
            string query = "DELETE FROM SavedGames_db WHERE ChatId = @ChatId AND GameName = @GameName";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ChatId", ChatId);
                command.Parameters.AddWithValue("@GameName", GameName);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}