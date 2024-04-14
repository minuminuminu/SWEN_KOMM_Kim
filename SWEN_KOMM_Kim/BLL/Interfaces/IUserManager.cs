using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.BLL.Interfaces
{
    internal interface IUserManager
    {
        User LoginUser(Credentials credentials);
        void RegisterUser(Credentials credentials);
        User GetUserByAuthToken(string authToken);
        User GetUserByUsername(string username);
        UserData GetUserData(string authToken);
        void UpdateUserData(UserData userData, string authToken);
    }
}
