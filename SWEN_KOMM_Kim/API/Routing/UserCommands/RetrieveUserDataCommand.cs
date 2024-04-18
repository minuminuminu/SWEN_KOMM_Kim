using Newtonsoft.Json;
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
    internal class RetrieveUserDataCommand : IRouteCommand
    {
        private readonly IUserController _userController;
        private readonly string _requestedUsername;
        private readonly User _requestingUser;

        public RetrieveUserDataCommand(IUserController userController, string requestedUsername, User requestingUser)
        {
            _userController = userController;
            _requestedUsername = requestedUsername;
            _requestingUser = requestingUser;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                var userToEdit = _userController.GetUserByUsername(_requestedUsername);

                if (_requestingUser.Token != "admin-sebToken" && _requestingUser.Token != userToEdit.Token)
                {
                    throw new UserNotAuthenticatedException();
                }

                var data = _userController.GetUserData(userToEdit.Token);
                var jsonPayload = JsonConvert.SerializeObject(data);
                response = new HttpResponse(StatusCode.Ok, jsonPayload);
            }
            catch (UserNotFoundException)
            {
                response = new HttpResponse(StatusCode.NotFound);
            }
            catch (UserNotAuthenticatedException)
            {
                response = new HttpResponse(StatusCode.Unauthorized);
            }

            return response;
        }
    }
}
