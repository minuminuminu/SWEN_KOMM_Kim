using Npgsql;
using SWEN_KOMM_Kim.DAL.Interfaces;
using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.DAL.DAOs
{
    internal class StatsDao : IStatsDao
    {
        private const string CreateUserStatsTableCommand = @"CREATE TABLE IF NOT EXISTS user_stats (elo int DEFAULT 100, wins int DEFAULT 0, losses int DEFAULT 0, authToken varchar REFERENCES users(authToken));";

        private const string SelectAllStatsEntriesCommand = @"SELECT ud.name, us.elo, us.wins, us.losses FROM user_stats us INNER JOIN user_data ud ON us.authToken = ud.authToken";
        private const string SelectUserStatsEntryCommand = @"SELECT ud.name, us.elo, us.wins, us.losses FROM user_stats us INNER JOIN user_data ud ON us.authToken = ud.authToken WHERE ud.authToken=@authToken ORDER BY us.elo DESC";

        private const string InsertUserStatsCommand = @"INSERT INTO user_stats (authToken) VALUES (@authToken)";

        private readonly string _connectionString;

        public StatsDao(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTables();
        }

        public bool CreateUserStatsEntry(string authToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(InsertUserStatsCommand, connection);
            cmd.Parameters.AddWithValue("authToken", authToken);
            var affectedRows = cmd.ExecuteNonQuery();

            return affectedRows > 0;
        }

        public UserStats? RetrieveUserStats(string authToken)
        {
            UserStats? stats = null;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(SelectUserStatsEntryCommand, connection);
            cmd.Parameters.AddWithValue("authToken", authToken);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string name = reader.GetString(reader.GetOrdinal("name"));
                int elo = reader.GetInt32(reader.GetOrdinal("elo"));
                int wins = reader.GetInt32(reader.GetOrdinal("wins"));
                int losses = reader.GetInt32(reader.GetOrdinal("losses"));

                stats = new UserStats(name, elo, wins, losses);
            }

            return stats;
        }

        public List<UserStats> RetrieveScoreboard()
        {
            List<UserStats> scoreboard = new List<UserStats>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(SelectAllStatsEntriesCommand, connection);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string name = reader.GetString(reader.GetOrdinal("name"));
                int elo = reader.GetInt32(reader.GetOrdinal("elo"));
                int wins = reader.GetInt32(reader.GetOrdinal("wins"));
                int losses = reader.GetInt32(reader.GetOrdinal("losses"));

                UserStats stats = new UserStats(name, elo, wins, losses);
                scoreboard.Add(stats);
            }

            return scoreboard;
        }

        private void EnsureTables()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(CreateUserStatsTableCommand, connection);
            cmd.ExecuteNonQuery();
        }
    }
}
