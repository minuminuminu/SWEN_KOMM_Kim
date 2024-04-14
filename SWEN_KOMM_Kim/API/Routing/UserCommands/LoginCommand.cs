using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.Exceptions;
using SWEN_KOMM_Kim.HttpServer.Response;
using SWEN_KOMM_Kim.HttpServer.Routing;
using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.API.Routing.UserCommands
{
    internal class LoginCommand : IRouteCommand
    {
        private readonly IUserManager _userManager;
        private readonly Credentials _credentials;

        public LoginCommand(IUserManager userManager, Credentials credentials)
        {
            _credentials = credentials;
            _userManager = userManager;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;
            User? user;

            try
            {
                user = _userManager.LoginUser(_credentials);
            }
            catch (UserNotFoundException)
            {
                user = null;
            }

            if (user == null)
            {
                response = new HttpResponse(StatusCode.Unauthorized);
            }
            else
            {
                response = new HttpResponse(StatusCode.Ok, user.Token);
            }

            return response;
        }
    }
}
