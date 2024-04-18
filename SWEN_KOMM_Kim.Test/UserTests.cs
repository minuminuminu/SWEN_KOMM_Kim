using SWEN_KOMM_Kim.Models;
using NUnit.Framework;
using System;
using Moq;
using SWEN_KOMM_Kim.DAL.Interfaces;
using SWEN_KOMM_Kim.DAL.DAOs;
using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.BLL.Managers;
using SWEN_KOMM_Kim.Exceptions;

namespace SWEN_KOMM_Kim.Test
{
    public class UserTests
    {
        private Mock<IUserDao> _mockUserDao;
        private IUserManager _userManager;

        [SetUp]
        public void Setup()
        {
            _mockUserDao = new Mock<IUserDao>();
            _userManager = new UserManager(_mockUserDao.Object);
        }

        [Test]
        public void UserHasValidToken()
        {
            // arrange
            var expectedToken = "testusr-sebToken";
            var user = new User("testusr", "test");

            // act
            var actualToken = user.Token;

            // assert
            Assert.That(actualToken, Is.EqualTo(expectedToken));
        }

        [Test]
        public void RegisterUser_FailureDueToDuplicate()
        {
            // Arrange
            _mockUserDao.Setup(x => x.InsertUser(It.IsAny<User>())).Returns(false);

            // Act & Assert
            Assert.Throws<DuplicateUserException>(() =>
                _userManager.RegisterUser(new Credentials("duplicateuser", "pass"))
            );
        }

        [Test]
        public void UserLogin_Successful()
        {
            // Arrange
            var expectedUser = new User("testuser", "testpass");
            _mockUserDao.Setup(x => x.GetUserByCredentials("testuser", "testpass")).Returns(expectedUser);

            // Act
            var result = _userManager.LoginUser(new Credentials("testuser", "testpass"));

            // Assert
            Assert.That(expectedUser, Is.EqualTo(result));
        }

        [Test]
        public void GetUserData_ValidToken_ReturnsUserData()
        {
            // Arrange
            var expectedData = new UserData("John Doe", "Biography here", "ImageURL");
            _mockUserDao.Setup(x => x.GetUserData("valid-token")).Returns(expectedData);

            // Act
            var result = _userManager.GetUserData("valid-token");

            // Assert
            Assert.That(expectedData, Is.EqualTo(result));
        }
    }
}