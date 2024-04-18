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
        IUserController _userController;
        IStatsController _statsController;
        Credentials _credentials;

        public RegisterCommand(IUserController userController, IStatsController statsController, Credentials credentials)
        {
            _userController = userController;
            _statsController = statsController;
            _credentials = credentials;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                var user = new User(_credentials.Username, _credentials.Password);

                _userController.RegisterUser(_credentials);
                _statsController.CreateUserStatsEntry(user.Token);
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
