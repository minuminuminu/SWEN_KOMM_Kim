using SWEN_KOMM_Kim.Models;
using NUnit.Framework;
using System;
using Moq;
using SWEN_KOMM_Kim.DAL.Interfaces;
using SWEN_KOMM_Kim.DAL.DAOs;
using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.BLL.Controllers;
using SWEN_KOMM_Kim.Exceptions;

namespace SWEN_KOMM_Kim.Test
{
    public class UserTests
    {
        private Mock<IUserDao> _mockUserDao;
        private IUserController _userController;

        [SetUp]
        public void Setup()
        {
            _mockUserDao = new Mock<IUserDao>();
            _userController = new UserController(_mockUserDao.Object);
        }

        [Test]
        public void UserHasValidToken()
        {
            var expectedToken = "testusr-sebToken";
            var user = new User("testusr", "test");

            var actualToken = user.Token;

            Assert.That(actualToken, Is.EqualTo(expectedToken));
        }

        [Test]
        public void RegisterUser_FailureDueToDuplicate()
        {
            _mockUserDao.Setup(x => x.InsertUser(It.IsAny<User>())).Returns(false);

            Assert.Throws<DuplicateUserException>(() =>
                _userController.RegisterUser(new Credentials("duplicateuser", "pass"))
            );
        }

        [Test]
        public void UserLogin_Successful()
        {
            var expectedUser = new User("testuser", "testpass");
            _mockUserDao.Setup(x => x.GetUserByCredentials("testuser", "testpass")).Returns(expectedUser);

            var result = _userController.LoginUser(new Credentials("testuser", "testpass"));

            Assert.That(expectedUser, Is.EqualTo(result));
        }

        [Test]
        public void GetUserData_ValidToken_ReturnsUserData()
        {
            var expectedData = new UserData("John Doe", "Biography here", "ImageURL");
            _mockUserDao.Setup(x => x.GetUserData("valid-token")).Returns(expectedData);

            var result = _userController.GetUserData("valid-token");

            Assert.That(expectedData, Is.EqualTo(result));
        }
    }
}