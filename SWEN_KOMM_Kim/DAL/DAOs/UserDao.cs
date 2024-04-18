using Npgsql;
using SWEN_KOMM_Kim.DAL.Interfaces;
using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.DAL.DAOs
{
    internal class UserDao : IUserDao
    {
        private const string CreateUserTableCommand = @"CREATE TABLE IF NOT EXISTS users (username varchar PRIMARY KEY, password varchar, authToken varchar UNIQUE);";
        private const string CreateUserDataTableCommand = @"CREATE TABLE IF NOT EXISTS user_data (name varchar DEFAULT NULL, bio varchar DEFAULT NULL, image varchar DEFAULT NULL, authToken varchar REFERENCES users(authToken) ON UPDATE CASCADE)";

        private const string SelectAllUsersCommand = @"SELECT * FROM users";
        private const string SelectUserByCredentialsCommand = "SELECT * FROM users WHERE username=@username AND password=@password";
        private const string SelectUserDataByUsernameCommand = @"SELECT * FROM user_data WHERE authToken=@authToken";

        private const string InsertUserCommand = @"INSERT INTO users(username, password, authToken) VALUES (@username, @password, @authToken)";
        private const string InsertDefaultUserDataCommand = @"INSERT INTO user_data(authToken) VALUES (@authToken)";

        private const string UpdateUserDataCommand = @"UPDATE user_data SET name=@name, bio=@bio, image=@image WHERE authToken=@authToken";
        private const string UpdateUserCredentialsCommand = @"UPDATE users SET username=@new_username, password=@password, authToken=@authToken WHERE username=@current_username";

        private readonly string _connectionString;

        public UserDao(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTables();
        }

        private void EnsureTables()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(CreateUserTableCommand, connection);
            cmd.ExecuteNonQuery();

            cmd.CommandText = CreateUserDataTableCommand;
            cmd.ExecuteNonQuery();
        }

        public bool InsertUser(User user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(InsertUserCommand, connection);
            cmd.Parameters.AddWithValue("username", user.Username);
            cmd.Parameters.AddWithValue("password", user.Password);
            cmd.Parameters.AddWithValue("authToken", user.Token);
            var affectedRows = cmd.ExecuteNonQuery();

            return affectedRows > 0;
        }

        public bool UpdateUserCredentials(User user, string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(UpdateUserCredentialsCommand, connection);
            cmd.Parameters.AddWithValue("new_username", user.Username);
            cmd.Parameters.AddWithValue("password", user.Password);
            cmd.Parameters.AddWithValue("authToken", user.Token);
            cmd.Parameters.AddWithValue("current_username", username);

            var affectedRows = cmd.ExecuteNonQuery();

            return affectedRows > 0;
        }

        public bool InsertNewUserData(string authToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(InsertDefaultUserDataCommand, connection);
            cmd.Parameters.AddWithValue("authToken", authToken);
            var affectedRows = cmd.ExecuteNonQuery();

            return affectedRows > 0;
        }

        public UserData? GetUserData(string authToken)
        {
            UserData? data = null;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(SelectUserDataByUsernameCommand, connection);
            cmd.Parameters.AddWithValue("authToken", authToken);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string name = reader.IsDBNull(reader.GetOrdinal("name")) ? "" : reader.GetString(reader.GetOrdinal("name"));
                string bio = reader.IsDBNull(reader.GetOrdinal("bio")) ? "" : reader.GetString(reader.GetOrdinal("bio"));
                string image = reader.IsDBNull(reader.GetOrdinal("image")) ? "" : reader.GetString(reader.GetOrdinal("image"));

                data = new UserData(name, bio, image);
            }

            return data;
        }

        public bool UpdateUserData(UserData userData, string authToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(UpdateUserDataCommand, connection);
            cmd.Parameters.AddWithValue("name", userData.Name);
            cmd.Parameters.AddWithValue("bio", userData.Bio);
            cmd.Parameters.AddWithValue("image", userData.Image);
            cmd.Parameters.AddWithValue("authToken", authToken);
            var affectedRows = cmd.ExecuteNonQuery();

            return affectedRows > 0;
        }

        public User? GetUserByCredentials(string username, string password)
        {
            User? user = null;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(SelectUserByCredentialsCommand, connection);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("password", password);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                user = ReadUser(reader);
            }

            return user;
        }

        public User? GetUserByAuthToken(string authToken)
        {
            return GetAllUsers().SingleOrDefault(u => u.Token == authToken);
        }

        public User? GetUserByUsername(string username)
        {
            return GetAllUsers().SingleOrDefault(u => u.Username == username);
        }

        private IEnumerable<User> GetAllUsers()
        {
            var users = new List<User>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(SelectAllUsersCommand, connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var user = ReadUser(reader);
                users.Add(user);
            }

            return users;
        }

        private User ReadUser(IDataRecord record)
        {
            var username = Convert.ToString(record["username"])!;
            var password = Convert.ToString(record["password"])!;

            return new User(username, password);
        }
    }
}
