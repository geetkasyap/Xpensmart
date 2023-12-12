using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using CoreModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpensmart.Controllers;

namespace Testing.NotificationTests
{
    public class NotificationControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ILogger<NotificationController>> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;
        private NotificationController _notificationController;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<NotificationController>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _notificationController = new NotificationController(_mockUnitOfWork.Object, _mockConfiguration.Object, _mockLogger.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _notificationController.Dispose();
        }


        [Test]
        public void AddNotifications_ReturnsSuccess_WhenValidRequest()
        {

            var notification = new NotificationsEntity { UserID = 1, Type = "Request", Message = "Test notification" };
            _mockUnitOfWork.Setup(u => u.Notification.AddNotifications(notification)).Returns(notification);


            var result = _notificationController.AddNotifications(notification) as GenericResponse;


            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public void AddNotifications_ReturnsFailure_WhenRequestIsNull()
        {

            NotificationsEntity notification = null;

            var result = _notificationController.AddNotifications(notification) as GenericResponse;

 
            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public void AddNotifications_ReturnsServerError_OnException()
        {

            var notification = new NotificationsEntity { UserID = 1, Type = "Request", Message = "Test notification" };
            _mockUnitOfWork.Setup(u => u.Notification.AddNotifications(notification)).Throws(new Exception("Test exception"));


            var result = _notificationController.AddNotifications(notification) as GenericResponse;


            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task GetnotificationDetails_ReturnsNotifications_WhenDataExists()
        {
     
            int userId = 1;
            var notifications = new List<NotificationsEntity>
            {
                new NotificationsEntity { NotificationID = 1, UserID = userId, Type = "Request", Message = "Test notification" }
            };
            _mockUnitOfWork.Setup(u => u.Notification.GetnotificationDetails(userId)).Returns(notifications);


            var result = await _notificationController.GetnotificationDetails(0, userId) as OkObjectResult;

  
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(notifications, result.Value);
        }

        [Test]
        public async Task GetnotificationDetails_ReturnsServerError_OnException()
        {

            int userId = 1;
            _mockUnitOfWork.Setup(u => u.Notification.GetnotificationDetails(userId)).Throws(new Exception("Test exception"));


            var result = await _notificationController.GetnotificationDetails(0, userId) as ObjectResult;


            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
        }

        [Test]
        public async Task ReadNotification_ReturnsSuccess_WhenValidId()
        {

            int notificationId = 1;
            var notification = new NotificationsEntity { NotificationID = notificationId, UserID = 1, Type = "Request", Message = "Test notification" };
            _mockUnitOfWork.Setup(u => u.Notification.ReadNotification(notificationId)).Returns(notification);


            var result = await _notificationController.ReadNotification(notificationId) as GenericResponse;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task ReadNotification_ReturnsServerError_OnException()
        {

            int notificationId = 1;
            _mockUnitOfWork.Setup(u => u.Notification.ReadNotification(notificationId)).Throws(new Exception("Test exception"));


            var result = await _notificationController.ReadNotification(notificationId) as GenericResponse;


            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task DeleteNotification_ReturnsSuccess_WhenValidId()
        {

            int notificationId = 1;
            var notification = new NotificationsEntity { NotificationID = notificationId, UserID = 1, Type = "Request", Message = "Test notification" };
            _mockUnitOfWork.Setup(u => u.Notification.DeleteNotification(notificationId)).Returns(notification);


            var result = await _notificationController.DeleteNotification(notificationId) as GenericResponse;


            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task DeleteNotification_ReturnsServerError_OnException()
        {

            int notificationId = 1;
            _mockUnitOfWork.Setup(u => u.Notification.DeleteNotification(notificationId)).Throws(new Exception("Test exception"));


            var result = await _notificationController.DeleteNotification(notificationId) as GenericResponse;


            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }
    }
}