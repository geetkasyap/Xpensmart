using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Utils;

namespace Testing.NotificationTests
{
    public class NotificationRepositoryTests:DbContextMock
    {

        [SetUp]
        public void Setup()
        {
            ReinitializeTest();
        }

        [Test]
        public void AddNotifications_Success()
        {
            var notification = new NotificationsEntity
            {
                UserID = 1,
                Type = "Info",
                Message = "Test Notification"
            };

            var result = _notificationRepository.AddNotifications(notification);

            Assert.IsNotNull(result);
            Assert.AreEqual(notification.Message, result.Message);
        }


        [Test]
        public void AddNotifications_Fail()
        {
            var notification = new NotificationsEntity
            {
                UserID = 1,
                Type = "Info",
                Message = "Test Notification"
            };

            mockContext.Setup(m => m.Set<NotificationsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _notificationRepository.AddNotifications(notification));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public void DeleteNotification_Success()
        {
            var result = _notificationRepository.DeleteNotification(1);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive);
        }
        [Test]
        public void DeleteNotification_Null_Success()
        {
            try
            {
                var result = _notificationRepository.DeleteNotification(100);

            }
            catch(Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Notification not found or already deleted."));
            }
        }

        [Test]
        public void DeleteNotification_Fail()
        {
            mockContext.Setup(m => m.Set<NotificationsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _notificationRepository.DeleteNotification(1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetnotificationDetails_Success()
        {
            var result = _notificationRepository.GetnotificationDetails(1);

            Assert.IsNotNull(result);
        }
        [Test]
        public void GetnotificationDetails_Fail()
        {
            mockContext.Setup(m => m.Set<NotificationsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _notificationRepository.GetnotificationDetails(1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetnotificationDetails_ById_Success()
        {
            var result = _notificationRepository.GetnotificationDetails(1, 1);

            Assert.IsNotNull(result);
        }

        [Test]
        public void GetnotificationDetails_ById_Fail()
        {
            mockContext.Setup(m => m.Set<NotificationsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _notificationRepository.GetnotificationDetails(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void ReadNotification_Success()
        {
            var result = _notificationRepository.ReadNotification(2);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ReadMessage);
        }
        [Test]
        public void ReadNotification_Null_Success()
        {
            try
            {
                var result = _notificationRepository.ReadNotification(500);
            }
            catch(Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Notification not found or already deleted."));

            }
        }
        [Test]
        public void ReadNotification_Fail()
        {
            mockContext.Setup(m => m.Set<NotificationsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _notificationRepository.ReadNotification(2));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }
    }
}