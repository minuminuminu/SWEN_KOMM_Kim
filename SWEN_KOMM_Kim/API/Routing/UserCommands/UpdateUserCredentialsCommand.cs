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
    internal class UpdateUserCredentialsCommand : IRouteCommand
    {
        private readonly IUserController _userController;
        private readonly User _updatedCredentials;
        private readonly User _requestingUser;
        private readonly string _userToUpdate;

        public UpdateUserCredentialsCommand(IUserController userController, User updatedCredentials, User requestingUser, string pathVar)
        {
            _userController = userController;
            _updatedCredentials = updatedCredentials;
            _requestingUser = requestingUser;
            _userToUpdate = pathVar;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                if(_requestingUser.Token != _userController.GetUserByUsername(_userToUpdate).Token && _requestingUser.Token != "admin-sebToken")
                {
                    throw new RouteNotAuthenticatedException();
                }

                _userController.UpdateUserCredentials(_updatedCredentials, _userToUpdate);
                response = new HttpResponse(StatusCode.Ok);
            }
            catch (RouteNotAuthenticatedException)
            {
                response = new HttpResponse(StatusCode.Unauthorized);
            }
            catch (DuplicateUserException)
            {
                response = new HttpResponse(StatusCode.Conflict);
            }

            return response;
        }
    }
}
