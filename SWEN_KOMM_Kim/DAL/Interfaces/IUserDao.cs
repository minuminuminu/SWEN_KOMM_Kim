using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.DAL.Interfaces
{
    internal interface IUserDao
    {
        User? GetUserByAuthToken(string authToken);
        User? GetUserByUsername(string username);
        User? GetUserByCredentials(string username, string password);
        bool InsertUser(User user);
        bool InsertNewUserData(string authToken);
        bool UpdateUserData(UserData userData, string authToken);
        UserData? GetUserData(string authToken);
    }
}
