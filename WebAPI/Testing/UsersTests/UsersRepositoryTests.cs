using CoreModels.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Utils;

namespace Testing.UsersTests
{
    public class UsersRepositoryTests:DbContextMock
    {
        [SetUp]
        public void Setup()
        {
            ReinitializeTest();
        }
        [Test]
        public void CreateUser()
        {
            UsersEntity usersEntity = new()
            {
                UserID = 1,
                UserName = "Kasyap",
                Email = "kasyap123@example.invalid",
                Password = "Sample@1234",
                FullName = "Kasyap",
                GroupID = 0,
                IsActive = true,
                BudgetsEntities = new List<BudgetsEntity>(),
                ExpenseProjectionEntities = new List<ExpenseProjectionEntity>(),
                ExpensesEntities = new List<ExpensesEntity>(),
                NotificationsEntities = new List<NotificationsEntity>(),
                ReportsEntities = new List<CategoryReport>(),
                SentRequest = new List<SharedAccountsEntity>(),
                ReceivedRequest = new List<SharedAccountsEntity>(),
                GroupMembersEntities = new List<GroupMembersEntity>(),
            };
            var output = _usersRepository.CreateUser(usersEntity);

            Assert.That(output, Is.Not.Null);
            mockContext.Verify(a => a.Set<UsersEntity>().Add(It.IsAny<UsersEntity>()), Times.Once);
            mockContext.Verify(a => a.SaveChanges(), Times.Once);
        }

        [Test]
        public void CreateUser_Failure()
        {
            var newUser = new UsersEntity
            {
                UserName = null,
                Email = "newuser@example.com",
                Password = "NewPassword",
                FullName = "New User"
            };
            mockContext.Setup(m => m.Set<UsersEntity>().Add(It.IsAny<UsersEntity>()));
            mockContext.Setup(m => m.SaveChanges()).Throws(new Exception("Database error"));
            var ex = Assert.Throws<Exception>(() => _usersRepository.CreateUser(newUser));
            Assert.That(ex.Message, Is.EqualTo("Database error"));

        }

        [Test]
        public void GetUserDetails_Success()
        {
            var result = _usersRepository.GetUserDetails();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
        [Test]
        public void GetUserDetails_Fail()
        {
            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () =>_usersRepository.GetUserDetails());
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetUserDetails_ByUserName_Success()
        {
            var result = _usersRepository.GetUserDetails("EncryptedUser");

            Assert.IsNotNull(result);
            Assert.AreEqual("EncryptedUser", result.UserName);
        }
        [Test]
        public void GetUserDetails_ByUserName_Fail()
        {
            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _usersRepository.GetUserDetails("EncryptedUser"));

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public void GetUserDetails_ByUserName_Failure()
        {
            var result = _usersRepository.GetUserDetails("NonExistentUser ");

            Assert.IsNull(result);
        }

        [Test]
        public void UpdateUser_Success()
        {
            var result = _usersRepository.UpdateUser("EncryptedUser", "EncryptedUser@123", "EncryptedUser@123");

            Assert.IsNotNull(result);
            Assert.AreEqual("7Mni12yAJKRZvaGrn8q/A4a+5CesYJ25embz2VcTwJk=", result.Password);
        }
        [Test]
        public void UpdateUser_WrongPassword_Success()
        {
            var result = _usersRepository.UpdateUser("EncryptedUser", "EncryptedUser@1234", "EncryptedUser@123");
            Assert.IsNull(result);
        }


        [Test]
        public void UpdateUser_Failure()
        {
            mockContext.Setup(m => m.Set<UsersEntity>().Add(It.IsAny<UsersEntity>()));
            mockContext.Setup(m => m.SaveChanges()).Throws(new Exception("Database error"));
            var ex = Assert.Throws<Exception>(() => _usersRepository.UpdateUser("EncryptedUsers", "EncryptedUser", "EncryptedUser"));
        }

        [Test]
        public void DeleteUser_Success()
        {
            var result = _usersRepository.DeleteUser("Geet");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive);
        }

        [Test]
        public void DeleteUser_Failure()
        {
            try
            {
                var result = _usersRepository.DeleteUser("NonExistentUser ");
                Assert.Throws<Exception>(() => _usersRepository.DeleteUser("NonExistentUser"));
            }
            catch(Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Object reference not set to an instance of an object."));
            }

        }

        [Test]
        public void CheckUserName_Success()
        {
            var result = _usersRepository.CheckUserName("Geet");

            Assert.IsNotNull(result);
            Assert.AreEqual("Geet", result.UserName);
        }
        [Test]
        public void CheckUserName_Fail()
        {
            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _usersRepository.CheckUserName("Geet"));

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void CheckUserName_Failure()
        {
            var result = _usersRepository.CheckUserName("NonExistentUser ");

            Assert.IsNull(result);
        }

        [Test]
        public void CheckEmail_Success()
        {
            var result = _usersRepository.CheckEmail("geet123@example.invalid");

            Assert.IsNotNull(result);
            Assert.AreEqual("Geet", result.UserName);
        }

        [Test]
        public void CheckEmail_Failure()
        {
            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _usersRepository.CheckEmail("nonexistent@example.com"));

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void LoginCheck_Success()
        {
            var result = _usersRepository.LoginCheck("encryptedUser123@example.invalid", "EncryptedUser@123");

            Assert.IsNotNull(result);
            Assert.AreEqual("EncryptedUser", result.UserName);
        }

        [Test]
        public void LoginCheck_Failure()
        {
            var result = _usersRepository.LoginCheck("geet123@example.invalid", "WrongPassword");

            Assert.IsNotNull(result);
        }
        [Test]
        public void LoginCheck_UnHashed_Failure()
        {
            var result = _usersRepository.LoginCheck("encryptedUser123@example.invalid", "lsadqoihzcbzkcougqowqiolshc;oxzbckadgiwf");

            Assert.IsNotNull(result);
        }
        [Test]
        public void LoginCheck_Fail()
        {
            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _usersRepository.LoginCheck("encryptedUser123@example.invalid", "EncryptedUser@123"));

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void ForgotPassword_Success()
        {
            var result = _usersRepository.ForgotPassword("encryptedUser123@example.invalid");

            Assert.IsNotNull(result);
            Assert.AreEqual("EncryptedUser@123", result);
        }
        [Test]
        public void ForgotPassword_Fail()
        {
            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _usersRepository.ForgotPassword("encryptedUser123@example.invalid"));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void ForgotPassword_Failure()
        {
            var result = _usersRepository.ForgotPassword("nonexistent@example.com");

            Assert.IsNull(result);
        }
        [Test]
        public void GetUserByUserNameAsync_Success()
        {
            var result = _usersRepository.GetUserByUserNameAsync("Test@1234");

            Assert.IsNotNull(result);
        }
        [Test]
        public void GetUserNamesByUserID_Success()
        {
            var result = _usersRepository.GetUserNamesByUserID(6);

            Assert.IsNotNull(result);
        }
        [Test]
        public void GetUserNamesByUserID_Failure()
        {
            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _usersRepository.GetUserNamesByUserID(10));

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }
        [Test]
        public void GetUserAllNames_Success()
        {
            var result = _usersRepository.GetUserAllNames();

            Assert.IsNotNull(result);
        }
        [Test]
        public void GetUserAllNames_Fail()
        {
            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _usersRepository.GetUserAllNames());

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }
        [Test]
        public void Encryption()
        {
            var result = _usersRepository.Encryption("test");
            Assert.AreEqual(result, "XS0hSabgau+pXonMwnHobw==");
        }

        [Test]
        public void Decryption()
        {
            var result = _usersRepository.Decryption("XS0hSabgau+pXonMwnHobw==");
            Assert.AreEqual(result, "test");
        }

        [Test]
        public void GetEmailByUserID_Success()
        {
            var result = _usersRepository.GetEmailByUserID(3);
            Assert.IsNotNull(result);
        }
        [Test]
        public void GetEmailByUserID_Fail()
        {
            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _usersRepository.GetEmailByUserID(1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

    }
}