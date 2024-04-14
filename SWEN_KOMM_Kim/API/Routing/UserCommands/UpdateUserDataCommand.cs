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
    internal class UpdateUserDataCommand : IRouteCommand
    {
        private readonly IUserManager _userManager;
        private readonly string _requestedUsername;
        private readonly User _requestingUser;
        private readonly UserData _userData;

        public UpdateUserDataCommand(IUserManager userManager, string requestedUsername, User requestingUser, UserData userData)
        {
            _userManager = userManager;
            _requestedUsername = requestedUsername;
            _requestingUser = requestingUser;
            _userData = userData;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                var userToEdit = _userManager.GetUserByUsername(_requestedUsername);

                if (_requestingUser.Token != "admin-sebToken" && _requestingUser.Token != userToEdit.Token)
                {
                    throw new UserNotAuthenticatedException();
                }

                _userManager.UpdateUserData(_userData, userToEdit.Token);
                response = new HttpResponse(StatusCode.Ok);
            }
            catch (UserNotAuthenticatedException)
            {
                response = new HttpResponse(StatusCode.Unauthorized);
            }
            catch (UserNotFoundException)
            {
                response = new HttpResponse(StatusCode.NotFound);
            }

            return response;
        }
    }
}
