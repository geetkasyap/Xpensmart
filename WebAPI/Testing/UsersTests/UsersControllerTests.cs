using ApplicationLayer.Interfaces;
using BusinessLayer;
using Castle.Core.Logging;
using CoreModels;
using CoreModels.Entities;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

namespace Testing.UsersTests
{
    public class UsersControllerTests
    {
        private UsersController _usersController;
        private Mock<IUsersRepository> _usersRepository;
        [OneTimeSetUp]
        public void SetUp()
        {

        }
        [SetUp]
        public void ReinitializeTest()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            var mockConfig = new Mock<IConfiguration>();
            var configSection = new Mock<IConfigurationSection>();
            var UsersLogger = new Mock<ILogger<UsersController>>();

            _usersController = new UsersController(unitOfWork.Object, mockConfig.Object, UsersLogger.Object);
            _usersRepository = new Mock<IUsersRepository>();
            unitOfWork.Setup(a => a.Users).Returns(_usersRepository.Object);

        }
        [Test]
        public async Task CreateUser()
        {
            var usersEntity = new UsersEntity
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
            _usersRepository.Setup(a => a.CreateUser(It.IsAny<UsersEntity>())).Returns(usersEntity);
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(a => a.Users).Returns(_usersRepository.Object);
            var result = await _usersController.CreateUser(usersEntity);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);

        }

        [Test]
        public async Task CreateUserNull()
        {
            UsersEntity usersEntity = null;
            _usersRepository.Setup(a => a.CreateUser(It.IsAny<UsersEntity>())).Returns(usersEntity);
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(a => a.Users).Returns(_usersRepository.Object);
            var result = await _usersController.CreateUser(usersEntity);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }
        [Test]
        public async Task CreateUser_Success()
        {
            var userEntity = new UsersEntity { UserName = "testUser " };

            _usersRepository.Setup(u => u.CreateUser(It.IsAny<UsersEntity>())).Returns(userEntity);

            var result = await _usersController.CreateUser(userEntity);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task CreateUser_NullEntity_Failure()
        {
            UsersEntity userEntity = null;

            var result = await _usersController.CreateUser(userEntity);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }
        [Test]
        public async Task GetUserDetails_Success()
        {
            var userEntity = new UsersEntity { UserName = "testUser " };
            _usersRepository.Setup(u => u.GetUserDetails("testUser ")).Returns(userEntity);

            var result = await _usersController.GetUserDetails("testUser ");

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(userEntity, okResult.Value);
        }

        [Test]
        public async Task GetUserDetails_UserNotFound_Failure()
        {
            _usersRepository.Setup(u => u.GetUserDetails("nonExistentUser ")).Returns((UsersEntity)null);

            var result = await _usersController.GetUserDetails("nonExistentUser ");
            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }
        [Test]
        public async Task GetUserDetails_UserNameNull_Failure()
        {
            var result = await _usersController.GetUserDetails(null);
            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(403, statusCodeResult.StatusCode);

        }

        [Test]
        public async Task GetUserDetailsList_Success()
        {
            var userEntity = new List<UsersEntity>
            {
                new UsersEntity
                {
                    UserID=1,
                    UserName="Geet",
                    Email="geet123@example.invalid",
                    Password="Test@1234",
                    FullName="Geet Kasyap",
                    GroupID=0,
                    IsActive=true,
                },
                new UsersEntity
                {
                    UserID=2,
                    UserName="Kasyap",
                    Email="kasyap123@example.invalid",
                    Password="Sample@1234",
                    FullName="Kasyap",
                    GroupID=0,
                    IsActive=true,
                },
            };
            _usersRepository.Setup(u => u.GetUserDetails()).Returns(userEntity);

            var result = await _usersController.GetUserDetails("All");

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(userEntity, okResult.Value);
        }

        [Test]
        public async Task GetUserDetailsList_AllNames_Success()
        {
            string[] List= { "Geet" , "Kasyap" };
            _usersRepository.Setup(u => u.GetUserAllNames()).Returns(List);

            var result = await _usersController.GetUserDetails("AllNames");

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(List, okResult.Value);
        }

        [Test]
        public async Task UpdateUser_Success()
        {
            var userEntity = new UsersEntity { UserName = "testUser " };
            _usersRepository.Setup(u => u.UpdateUser("testUser ", "oldPassword", "newPassword")).Returns(userEntity);

            var result = await _usersController.UpdateUser("testUser ", "oldPassword", "newPassword");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task UpdateUser_UserNotFound_Failure()
        {
            _usersRepository.Setup(u => u.UpdateUser("nonExistentUser ", "oldPassword", "newPassword")).Returns((UsersEntity)null);

            var result = await _usersController.UpdateUser("nonExistentUser ", "oldPassword", "newPassword");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task DeleteUser_Success()
        {
            _usersRepository.Setup(u => u.DeleteUser("testUser ")).Returns(new UsersEntity());

            var result = await _usersController.DeleteUser("testUser ");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task DeleteUser_UserNotFound_Failure()
        {
            _usersRepository.Setup(u => u.DeleteUser("nonExistentUser")).Throws(new Exception());

            var result = await _usersController.DeleteUser("nonExistentUser");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task CheckUserName_Success()
        {
            var userEntity = new UsersEntity { UserName = "testUser " };
            _usersRepository.Setup(u => u.CheckUserName("testUser ")).Returns(userEntity);
            var result = await _usersController.CheckUserName("testUser ");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }
        [Test]
        public async Task CheckUserName_Null_Success()
        {
            var result = await _usersController.CheckUserName(null);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }


        [Test]
        public async Task CheckUserName_UserNotFound_Failure()
        {
            _usersRepository.Setup(u => u.CheckUserName("nonExistentUser ")).Returns((UsersEntity)null);
            var result = await _usersController.CheckUserName("nonExistentUser ");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task CheckEmail_Success()
        {
            var userEntity = new UsersEntity { Email = "test@example.com" };
            _usersRepository.Setup(u => u.CheckEmail("test@example.com")).Returns(userEntity);
            var result = await _usersController.CheckEmail("test@example.com");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task CheckEmail_Null_Success()
        {
            var result = await _usersController.CheckEmail(null);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task CheckEmail_EmailNotFound_Failure()
        {
            _usersRepository.Setup(u => u.CheckEmail("nonExistent@example.com")).Returns((UsersEntity)null);
            var result = await _usersController.CheckEmail("nonExistent@example.com");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task LoginCheck_Success()
        {
            var userEntity = new UsersEntity { UserName = "testUser ", Email = "test@example.com" };
            _usersRepository.Setup(u => u.LoginCheck("test@example.com", "password")).Returns(userEntity);
            var result = await _usersController.LoginCheck("test@example.com", "password");

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(userEntity, okResult.Value);
        }

        [Test]
        public async Task LoginCheck_InvalidCredentials_Failure()
        {
            _usersRepository.Setup(u => u.LoginCheck("test@example.com", "wrongPassword")).Returns((UsersEntity)null);

            var result = await _usersController.LoginCheck("test@example.com", "wrongPassword");

            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }

        [Test]
        public async Task ForgotPassword_Success()
        {
            string email = "test@example.com";
            string password = "newPassword";
            _usersRepository.Setup(u => u.ForgotPassword(email)).Returns(password);
            var result = await _usersController.ForgotPassoword(email);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
            Assert.AreEqual(password, result.data);
        }

        [Test]
        public async Task ForgotPassword_EmailNotFound_Failure()
        {
            string email = "nonExistent@example.com";
            _usersRepository.Setup(u => u.ForgotPassword(email)).Returns((string)null);

            var result = await _usersController.ForgotPassoword(email);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        // Additional tests for exception handling
        [Test]
        public async Task CreateUser_Exception_Failure()
        {
            var userEntity = new UsersEntity { UserName = "testUser " };
            _usersRepository.Setup(u => u.CreateUser(It.IsAny<UsersEntity>())).Throws(new Exception("Database error"));

            var result = await _usersController.CreateUser(userEntity);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task GetUserDetails_Exception_Failure()
        {
            _usersRepository.Setup(u => u.GetUserDetails("testUser ")).Throws(new Exception("Database error"));

            var result = await _usersController.GetUserDetails("testUser ");

            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task UpdateUser_Exception_Failure()
        {
            _usersRepository.Setup(u => u.UpdateUser("testUser ", "oldPassword", "newPassword")).Throws(new Exception("Database error"));

            var result = await _usersController.UpdateUser("testUser ", "oldPassword", "newPassword");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task DeleteUser_Exception_Failure()
        {
            _usersRepository.Setup(u => u.DeleteUser("testUser ")).Throws(new Exception("Database error"));

            var result = await _usersController.DeleteUser("testUser ");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task CheckUserName_Exception_Failure()
        {
            _usersRepository.Setup(u => u.CheckUserName("testUser ")).Throws(new Exception("Database error"));

            var result = await _usersController.CheckUserName("testUser ");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task CheckEmail_Exception_Failure()
        {
            _usersRepository.Setup(u => u.CheckEmail("test@example.com")).Throws(new Exception("Database error"));

            var result = await _usersController.CheckEmail("test@example.com");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task LoginCheck_Exception_Failure()
        {
            _usersRepository.Setup(u => u.LoginCheck("test@example.com", "password")).Throws(new Exception("Database error"));

            var result = await _usersController.LoginCheck("test@example.com", "password");

            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task ForgotPassword_Exception_Failure()
        {
            _usersRepository.Setup(u => u.ForgotPassword("test@example.com")).Throws(new Exception("Database error"));

            var result = await _usersController.ForgotPassoword("test@example.com");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }
        [Test]
        [TestCase(1)]
        public async Task GetUserNamesByUserID_Success(int UserID)
        {
                string Name = "testUser ";
                _usersRepository.Setup(u => u.GetUserNamesByUserID(UserID)).Returns(Name);
                var result = await _usersController.GetUserNamesByUserID(UserID);
                Assert.IsNotNull(result);
            
        }
        [Test]
        [TestCase(0)]
        public async Task GetUserNamesByUserID_Zero_Success(int UserID)
        {
                string Name = "testUser ";
                var result = await _usersController.GetUserNamesByUserID(UserID);
                Assert.IsInstanceOf<ObjectResult>(result);
                var statusCodeResult = result as ObjectResult;
                Assert.AreEqual(403, statusCodeResult.StatusCode);
        }
        [Test]
        public async Task GetUserNamesByUserID_Fail()
        {
            _usersRepository.Setup(u => u.GetUserNamesByUserID(999)).Throws(new Exception("Database error"));
            var result = await _usersController.GetUserNamesByUserID(999);
            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }
        [Test]
        public async Task CreateSharedUser_Null_Success()
        {
            SharedAccountsEntity sharedAccountsEntity = null;
            var result = _usersController.CreateSharedUser(sharedAccountsEntity);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);

        }
        [Test]
        public async Task CreateUser_Ex_Failure()
        {
            var userEntity = new UsersEntity { UserName = "testUser  " };
            _usersRepository.Setup(u => u.CreateUser(It.IsAny<UsersEntity>())).Throws(new Exception("Database error"));

            var result = await _usersController.CreateUser(userEntity);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }
        [Test]
        public async Task GetUserDetails_AllUsers_Success()
        {
            var userEntities = new List<UsersEntity>
    {
        new UsersEntity { UserName = "User 1" },
        new UsersEntity { UserName = "User 2" }
    };
            _usersRepository.Setup(u => u.GetUserDetails()).Returns(userEntities);

            var result = await _usersController.GetUserDetails("All");

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(userEntities, okResult.Value);
        }

        [Test]
        public async Task GetUserDetails_AllNames_Success()
        {
            string[] userNames = { "User 1", "User 2" };
            _usersRepository.Setup(u => u.GetUserAllNames()).Returns(userNames);

            var result = await _usersController.GetUserDetails("AllNames");

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(userNames, okResult.Value);
        }
        [Test]
        public async Task UpdateUser_UserNotFound_Fail()
        {
            _usersRepository.Setup(u => u.UpdateUser("nonExistentUser ", "oldPassword", "newPassword")).Returns((UsersEntity)null);

            var result = await _usersController.UpdateUser("nonExistentUser ", "oldPassword", "newPassword");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task CheckUserName_Exists_Failure()
        {
            var userEntity = new UsersEntity { UserName = "existingUser " };
            _usersRepository.Setup(u => u.CheckUserName("existingUser ")).Returns(userEntity);

            var result = await _usersController.CheckUserName("existingUser ");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task CheckEmail_Exists_Failure()
        {
            var userEntity = new UsersEntity { Email = "existing@example.com" };
            _usersRepository.Setup(u => u.CheckEmail("existing@example.com")).Returns(userEntity);

            var result = await _usersController.CheckEmail("existing@example.com");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

    }
}


