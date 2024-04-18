using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.HttpServer.Request;
using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.API
{
    internal class IdentityProvider
    {
        private readonly IUserController _userController;

        public IdentityProvider(IUserController userController)
        {
            _userController = userController;
        }

        public User? GetIdentityForRequest(HttpRequest request)
        {
            User? currentUser = null;

            if (request.Header.TryGetValue("Authorization", out var authToken))
            {
                const string prefix = "Basic ";
                if (authToken.StartsWith(prefix))
                {
                    try
                    {
                        currentUser = _userController.GetUserByAuthToken(authToken.Substring(prefix.Length));
                    }
                    catch { }
                }
            }

            return currentUser;
        }
    }
}
