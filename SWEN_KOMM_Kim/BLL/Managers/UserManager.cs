using Npgsql;
using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.DAL.Interfaces;
using SWEN_KOMM_Kim.Exceptions;
using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.BLL.Managers
{
    internal class UserManager : IUserManager
    {
        private readonly IUserDao _userDao;

        public UserManager(IUserDao userDao)
        {
            _userDao = userDao;
        }

        public User GetUserByAuthToken(string authToken)
        {
            return _userDao.GetUserByAuthToken(authToken) ?? throw new UserNotFoundException();
        }

        public User GetUserByUsername(string username)
        {
            return _userDao.GetUserByUsername(username) ?? throw new UserNotFoundException();
        }

        public User LoginUser(Credentials credentials)
        {
            return _userDao.GetUserByCredentials(credentials.Username, credentials.Password) ?? throw new UserNotFoundException();
        }

        public void RegisterUser(Credentials credentials)
        {
            var user = new User(credentials.Username, credentials.Password);

            try
            {
                if(!_userDao.InsertUser(user))
                {
                    throw new DuplicateUserException();
                }
                _userDao.InsertNewUserData(user.Token); // user data table
            } 
            catch(PostgresException ex) when (ex.SqlState == "23505") // duplicate pkey violation error code
            {
                throw new DuplicateUserException();
            }
        }

        public void UpdateUserCredentials(User user, string username)
        {
            try
            {
                if (!_userDao.UpdateUserCredentials(user, username))
                {
                    throw new DuplicateUserException();
                }
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                throw new DuplicateUserException();
            }
        }

        public UserData GetUserData(string authToken)
        {
            var data = _userDao.GetUserData(authToken);

            if(data == null)
            {
                throw new UserNotFoundException();
            }

            return data;
        }

        public void UpdateUserData(UserData userData, string authToken)
        {
            if(!_userDao.UpdateUserData(userData, authToken))
            {
                throw new UserNotFoundException();
            }
        }
    }
}
