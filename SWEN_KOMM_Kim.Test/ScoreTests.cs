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
    internal class ScoreTests
    {
        private Mock<IStatsDao> _mockStatsDao;
        private IStatsManager _statsManager;

        [SetUp]
        public void Setup()
        {
            _mockStatsDao = new Mock<IStatsDao>();
            _statsManager = new StatsManager(_mockStatsDao.Object);
        }

        [Test]
        public void CreateUserStatsEntry_Success()
        {
            // Arrange
            string authToken = "someAuthToken";

            // Act
            _statsManager.CreateUserStatsEntry(authToken);

            // Assert
            _mockStatsDao.Verify(dao => dao.CreateUserStatsEntry(authToken), Times.Once);
        }

        [Test]
        public void IncreaseStats_Success()
        {
            // Arrange
            int elo = 50;
            int pushups = 10;
            string authToken = "someAuthToken";

            // Act
            _statsManager.IncreaseStats(elo, pushups, authToken);

            // Assert
            _mockStatsDao.Verify(dao => dao.IncreaseStats(elo, pushups, authToken), Times.Once);
        }

        [Test]
        public void RetrieveUserStats_UserNotFound()
        {
            // Arrange
            string authToken = "nonExistingAuthToken";
            _mockStatsDao.Setup(dao => dao.RetrieveUserStats(authToken)).Returns((UserStats)null);

            // Act & Assert
            Assert.Throws<UserNotFoundException>(() => _statsManager.RetrieveUserStats(authToken));
        }

        [Test]
        public void RetrieveUserStats_Success()
        {
            // Arrange
            string authToken = "existingAuthToken";
            var expectedStats = new UserStats("testUser", 100, 0);
            _mockStatsDao.Setup(dao => dao.RetrieveUserStats(authToken)).Returns(expectedStats);

            // Act
            var result = _statsManager.RetrieveUserStats(authToken);

            // Assert
            Assert.That(expectedStats, Is.EqualTo(result));
        }
    }
}
