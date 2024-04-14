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
    internal class RegisterCommand : IRouteCommand
    {
        IUserManager _userManager;
        IStatsManager _statsManager;
        Credentials _credentials;

        public RegisterCommand(IUserManager userManager, IStatsManager statsManager, Credentials credentials)
        {
            _userManager = userManager;
            _statsManager = statsManager;
            _credentials = credentials;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                var user = new User(_credentials.Username, _credentials.Password);

                _userManager.RegisterUser(_credentials);
                _statsManager.CreateUserStatsEntry(user.Token);
                response = new HttpResponse(StatusCode.Created);
            }
            catch (DuplicateUserException)
            {
                response = new HttpResponse(StatusCode.Conflict);
            }

            return response;
        }
    }
}
