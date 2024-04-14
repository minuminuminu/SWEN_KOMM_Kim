using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.Models
{
    internal class User
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Token => $"{Username}-sebToken";

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
