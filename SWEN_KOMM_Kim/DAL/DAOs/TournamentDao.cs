using Npgsql;
using SWEN_KOMM_Kim.DAL.Interfaces;
using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SWEN_KOMM_Kim.DAL.DAOs
{
    internal class TournamentDao : ITournamentDao
    {
        private readonly ConcurrentDictionary<string, Tournament> _tournaments = new ConcurrentDictionary<string, Tournament>();

        private readonly string CreateHistoryTableCommand = @"CREATE TABLE IF NOT EXISTS history (id SERIAL PRIMARY KEY, count int, duration int, username varchar references users(username) ON UPDATE CASCADE)";

        private readonly string SelectAllEntriesByUsernameCommand = @"SELECT * FROM history WHERE username=@username";

        private readonly string InsertHistoryEntryCommand = @"INSERT INTO history (count, duration, username) VALUES (@count, @duration, @username)";

        private readonly string _connectionString;

        public TournamentDao(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTables();
        }

        private void EnsureTables()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(CreateHistoryTableCommand, connection);
            cmd.ExecuteNonQuery();
        }

        public bool InsertHistoryEntry(TournamentEntry entry)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(InsertHistoryEntryCommand, connection);
            cmd.Parameters.AddWithValue("count", entry.Count);
            cmd.Parameters.AddWithValue("duration", entry.Duration);
            cmd.Parameters.AddWithValue("username", entry.Username);
            var affectedRows = cmd.ExecuteNonQuery();

            return affectedRows > 0;
        }

        public List<TournamentEntry> GetUserHistory(string username)
        {
            List<TournamentEntry> entries = new List<TournamentEntry>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(SelectAllEntriesByUsernameCommand, connection);
            cmd.Parameters.AddWithValue("username", username);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var count = reader.GetInt32(reader.GetOrdinal("count"));
                var duration = reader.GetInt32(reader.GetOrdinal("duration"));
                TournamentEntry entry = new TournamentEntry(count, duration, username);

                entries.Add(entry);
            }

            return entries;
        }

        public bool IsUserInTournament(string username)
        {
            foreach (var tournament in _tournaments.Values)
            {
                foreach (var entry in tournament.Entries)
                {
                    if (entry.Username == username)
                    {
                        return true; 
                    }
                }
            }

            return false; 
        }

        public string? GetTournamentNameByUsername(string username)
        {
            foreach (var tournamentEntry in _tournaments)
            {
                var tournamentName = tournamentEntry.Key;
                var tournament = tournamentEntry.Value;

                foreach (var entry in tournament.Entries)
                {
                    if (entry.Username == username)
                    {
                        return tournamentName; 
                    }
                }
            }

            return null; 
        }

        public Tournament? GetTournamentByName(string tournamentName)
        {
            _tournaments.TryGetValue(tournamentName, out Tournament tournament);
            return tournament;
        }

        public bool InsertEntryInMemoryDB(TournamentEntry entry, string tournamentName)
        {
            if (_tournaments.TryGetValue(tournamentName, out Tournament tournament))
            {
                tournament.Entries.Add(entry);
                return true;
            }
            return false;
        }

        public bool InsertTournamentInMemoryDB(Tournament tournament, string tournamentName)
        {
            return _tournaments.TryAdd(tournamentName, tournament);
        }

        public bool RemoveTournamentFromMemoryDB(string tournamentName)
        {
            return _tournaments.TryRemove(tournamentName, out _);
        } // discard "_" -> normalerweise 2. param outputted das element, das entfernt wurde
        // "out _" -> sagt, dass uns das hierbei egal ist, nur die operation muss erfolgen

        public bool DoesTournamentExist(string tournamentName)
        {
            if (_tournaments.ContainsKey(tournamentName))
            {
                return true;
            }

            return false;
        }
    }
}
