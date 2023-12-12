using ApplicationLayer.Interfaces;
using CoreModels.DTO;
using CoreModels.Entities;
using CoreModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpensmart.Controllers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer;
using Microsoft.Extensions.Configuration;
using DataAccessLayer.Infrastructure;

namespace Testing.GroupsTests
{
    public class GroupsControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ILogger<GroupsController>> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;
        private GroupsController _groupsController;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<GroupsController>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _groupsController = new GroupsController(_mockUnitOfWork.Object, _mockConfiguration.Object, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _groupsController.Dispose();
        }

        [Test]
        public async Task CreateGroup_ReturnsSuccess_WhenValidRequest()
        {
            int userId = 5;
            var request = new GroupCreationRequest { GroupName = "Test Group", UserIds = new List<int> { 1,2,5 } };
            var newGroup = new GroupsEntity {
                GroupID = 6,
                GroupName = "Fail Trip",
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "encryptedUser123@example.invalid",
                ModifiedBy = "encryptedUser123@example.invalid"
            };
            var user = new UsersEntity { Email = "encryptedUser123@example.invalid" };
            _mockUnitOfWork.Setup(u => u.Users.GetUserNamesByUserID(userId)).Returns("EncryptedUser");
            _mockUnitOfWork.Setup(u => u.Users.GetUserDetails("EncryptedUser")).Returns(user);
            _mockUnitOfWork.Setup(u => u.Groups.CreateGroupAsync(It.IsAny<GroupsEntity>())).ReturnsAsync(newGroup);
            _mockUnitOfWork.Setup(u => u.Groups.AddGroupMembersAsync(It.IsAny<List<GroupMembersEntity>>())).Returns(Task.CompletedTask);

            var result = await _groupsController.CreateGroup(userId, request);


            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task CreateGroup_ReturnsFailure_WhenRequestIsNull()
        {

            int userId = 1;
            GroupCreationRequest request = null;


            var result = await _groupsController.CreateGroup(userId, request);


            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task CreateGroup_ReturnsSuccess_WhenValidRequest_allNull()
        {
            int userId = 5;
            var request = new GroupCreationRequest { GroupName = null, UserIds = new List<int> { 1, 2, 5 } };
            var newGroup = new GroupsEntity
            {
                GroupID = 6,
                GroupName = null,
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "encryptedUser123@example.invalid",
                ModifiedBy = "encryptedUser123@example.invalid"
            };
            var user = new UsersEntity { Email = "encryptedUser123@example.invalid" };
            _mockUnitOfWork.Setup(u => u.Users.GetUserNamesByUserID(userId)).Returns("EncryptedUser");
            _mockUnitOfWork.Setup(u => u.Users.GetUserDetails("EncryptedUser")).Returns(user);
            _mockUnitOfWork.Setup(u => u.Groups.CreateGroupAsync(It.IsAny<GroupsEntity>())).ReturnsAsync(newGroup);
            _mockUnitOfWork.Setup(u => u.Groups.AddGroupMembersAsync(It.IsAny<List<GroupMembersEntity>>())).Returns(Task.CompletedTask);

            var result = await _groupsController.CreateGroup(userId, request);


            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }


        [Test]
        public async Task CreateGroup_ReturnsServerError_OnException()
        {

            int userId = 1;
            var request = new GroupCreationRequest { GroupName = "Test Group", UserIds = new List<int> { 1, 2 } };
            _mockUnitOfWork.Setup(u => u.Users.GetUserNamesByUserID(userId)).Throws(new Exception("Test exception"));


            var result = await _groupsController.CreateGroup(userId, request) as GenericResponse;


            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task GetGroupsForUser_ReturnsGroups_WhenDataExists()
        {

            int userId = 1;
            var groups = new List<GroupsEntity>
            {
                new GroupsEntity { GroupID = 1, GroupName = "Group 1" },
                new GroupsEntity { GroupID = 2, GroupName = "Group 2" }
            };
            _mockUnitOfWork.Setup(u => u.Groups.GetGroupsForUserAsync(userId)).ReturnsAsync(groups);


            var result = await _groupsController.GetGroupsForUser(userId) as OkObjectResult;


            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(groups, result.Value);
        }

        [Test]
        public async Task GetGroupsForUser_ReturnsServerError_OnException()
        {

            int userId = 1;
            _mockUnitOfWork.Setup(u => u.Groups.GetGroupsForUserAsync(userId)).Throws(new Exception("Test exception"));


            var result = await _groupsController.GetGroupsForUser(userId) as ObjectResult;


            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
        }



        [Test]
        public async Task GetExpensesForGroup_Success()
        {

            int groupId = 1;
            var expenses = new List<ExpensesEntity>
            {
                new ExpensesEntity { ExpenseID = 10,
                    UserID = 5,
                    Amount = 1000,
                    Date = DateTime.Now,
                    CategoryID = 2,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "Need To Be Paid",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "encryptedUser123@example.invalid",
                    ModifiedBy = "encryptedUser123@example.invalid" },
            };
            _mockUnitOfWork.Setup(u => u.Groups.GetExpensesForGroupAsync(groupId)).ReturnsAsync(expenses);
            var result = await _groupsController.GetExpensesForGroup(groupId);

            Assert.IsNotNull(result);
        }
        [Test]
        public async Task GetExpensesForGroup_Fail()
        {

            int groupId = 10;
            var expenses = new List<ExpensesEntity>
            {
                new ExpensesEntity { ExpenseID = 10,
                    UserID = 5,
                    Amount = 1000,
                    Date = DateTime.Now,
                    CategoryID = 2,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "Need To Be Paid",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "encryptedUser123@example.invalid",
                    ModifiedBy = "encryptedUser123@example.invalid" },
            };


            _mockUnitOfWork.Setup(u => u.Groups.GetExpensesForGroupAsync(groupId)).Throws(new Exception("Database error"));
            var result = await _groupsController.GetExpensesForGroup(groupId);
            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }



        [Test]
        public async Task GetExpensesForGroup_OnException()
        {

            int userId = 1;
            _mockUnitOfWork.Setup(u => u.Groups.GetGroupsForUserAsync(userId)).Throws(new Exception("Test exception"));


            var result = await _groupsController.GetGroupsForUser(userId) as ObjectResult;


            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
        }

        [Test]
        public async Task AddGroupExpense_ReturnsSuccess_WhenValidRequest()
        {
            int userId = 5;
            var request = new GroupExpenseRequest
            {
                GroupID = 1,
                Expense = new ExpensesEntity
                {
                    Amount = 100,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    Description = "Test Expense",
                    ExpenseType = "Business"
                }
            };

            var user = new UsersEntity { Email = "encryptedUser 123@example.invalid" };
            var addedExpense = new ExpensesEntity { ExpenseID = 1 };

            _mockUnitOfWork.Setup(u => u.Users.GetUserNamesByUserID(userId)).Returns("EncryptedUser ");
            _mockUnitOfWork.Setup(u => u.Users.GetUserDetails("EncryptedUser ")).Returns(user);
            _mockUnitOfWork.Setup(u => u.Expenses.AddExpensesAsync(It.IsAny<ExpensesEntity>())).ReturnsAsync(addedExpense);
            _mockUnitOfWork.Setup(u => u.Groups.AddGroupExpensesAsync(It.IsAny<GroupExpensesEntity>())).Returns(Task.CompletedTask);

            var result = await _groupsController.AddGroupExpense(userId, request);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task AddGroupExpense_ReturnsFailure_WhenRequestIsNull()
        {
            int userId = 5;
            GroupExpenseRequest request = null;

            var result = await _groupsController.AddGroupExpense(userId, request);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task AddGroupExpense_ReturnsServerError_OnException()
        {
            int userId = 5;
            var request = new GroupExpenseRequest
            {
                GroupID = 1,
                Expense = new ExpensesEntity
                {
                    Amount = 100,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    Description = "Test Expense",
                    ExpenseType = "Business"
                }
            };

            _mockUnitOfWork.Setup(u => u.Users.GetUserNamesByUserID(userId)).Throws(new Exception("Test exception"));

            var result = await _groupsController.AddGroupExpense(userId, request) as GenericResponse;

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task AddReconciliationExpense_ReturnsSuccess_WhenValidRequest()
        {
            int userId = 5;
            var request = new ReconciliationExpenseRequest
            {
                GroupID = 1,
                TotalAmount = 100,
                UserExpenses = new List<UserExpense>
        {
            new UserExpense { UserID = 1, Amount = 50 },
            new UserExpense { UserID = 2, Amount = 50 }
        },
                Date = DateTime.Now,
                CategoryID = 1,
                FileUrl = "url"
            };

            var user = new UsersEntity { Email = "encryptedUser 123@example.invalid" };
            _mockUnitOfWork.Setup(u => u.Users.GetUserNamesByUserID(userId)).Returns("EncryptedUser ");
            _mockUnitOfWork.Setup(u => u.Users.GetUserDetails("EncryptedUser ")).Returns(user);
            _mockUnitOfWork.Setup(u => u.Expenses.AddExpensesAsync(It.IsAny<ExpensesEntity>())).ReturnsAsync(new ExpensesEntity { ExpenseID = 1 });
            _mockUnitOfWork.Setup(u => u.Groups.AddGroupExpensesAsync(It.IsAny<GroupExpensesEntity>())).Returns(Task.CompletedTask);

            var result = await _groupsController.AddReconciliationExpense(userId, request);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task AddReconciliationExpense_ReturnsFailure_WhenRequestIsNull()
        {
            int userId = 5;
            ReconciliationExpenseRequest request = null;

            var result = await _groupsController.AddReconciliationExpense(userId, request);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task GetUsersInGroup_ReturnsUsers_WhenDataExists()
        {
            int groupId = 1;
            var users = new List<GroupMember>
    {
        new GroupMember { UserID = 1, UserName = "User  1" },
        new GroupMember { UserID = 2, UserName = "User  2" }
    };
            _mockUnitOfWork.Setup(u => u.Groups.GetUsersInGroupAsync(groupId)).ReturnsAsync(users);

            var result = await _groupsController.GetUsersInGroup(groupId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(users, result.Value);
        }

        [Test]
        public async Task GetUsersInGroup_ReturnsServerError_OnException()
        {
            int groupId = 1;
            _mockUnitOfWork.Setup(u => u.Groups.GetUsersInGroupAsync(groupId)).Throws(new Exception("Test exception"));

            var result = await _groupsController.GetUsersInGroup(groupId) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
        }

        [Test]
        public async Task SettleGroupExpenses_ReturnsSuccess_WhenValidRequest()
        {
            int userId = 5;
            var request = new SettlementRequest
            {
                GroupID = 1,
                UserSettlements = new List<UserSettlement>
        {
            new UserSettlement { UserID = 1, TotalOwed = 50 },
            new UserSettlement { UserID = 2, TotalOwed = -50 }
        }
            };

            _mockUnitOfWork.Setup(u => u.Groups.SettleGroupExpensesAsync(request.GroupID, request.UserSettlements)).Returns(Task.CompletedTask);

            var result = await _groupsController.SettleGroupExpenses(userId, request);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task SettleGroupExpenses_ReturnsFailure_WhenRequestIsNull()
        {
            int userId = 99;
            var request = new SettlementRequest
            {
                GroupID = 1,
                UserSettlements = new List<UserSettlement>
        {
            new UserSettlement { UserID = 1, TotalOwed = 50 },
            new UserSettlement { UserID = 2, TotalOwed = -50 }
        }
            };

            _mockUnitOfWork.Setup(u => u.Groups.SettleGroupExpensesAsync(request.GroupID, request.UserSettlements)).Throws(new Exception ());

            var result = await _groupsController.SettleGroupExpenses(userId, request);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task GetUserSettlementDetails_ReturnsSettlementDetails_WhenDataExists()
        {
            int groupId = 1;
            var users = new List<GroupMember>
    {
        new GroupMember { UserID = 1, UserName = "User  1" },
        new GroupMember { UserID = 2, UserName = "User  2" }
    };
            var expenses = new List<ExpensesEntity>
    {
        new ExpensesEntity { ExpenseID = 1, Amount = 100, UserID = 1 },
        new ExpensesEntity { ExpenseID = 2, Amount = 50, UserID = 2 }
    };

            _mockUnitOfWork.Setup(u => u.Groups.GetUsersInGroupAsync(groupId)).ReturnsAsync(users);
            _mockUnitOfWork.Setup(u => u.Groups.GetExpensesForGroupAsync(groupId)).ReturnsAsync(expenses);

            var result = await _groupsController.GetUserSettlementDetails(groupId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetUserSettlementDetails_ReturnsServerError_OnException()
        {
            int groupId = 1;
            _mockUnitOfWork.Setup(u => u.Groups.GetUsersInGroupAsync(groupId)).Throws(new Exception("Test exception"));

            var result = await _groupsController.GetUserSettlementDetails(groupId);
            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task DeleteGroup_ReturnsSuccess_WhenGroupCanBeDeleted()
        {
            int groupId = 1;
            var settlementDetails = new List<UserBalance>
        {
            new UserBalance { UserID = 1, UserName = "User  1", NetBalance = 0 },
            new UserBalance { UserID = 2, UserName = "User  2", NetBalance = 0 }
        };
            var GroupMembers = new List<GroupMember>
        {
             new GroupMember { UserID = 1, UserName="Geet",Email="geet123@example.invalid"}
        };
            var ExpenseList = new List<ExpensesEntity>
        {
            new ExpensesEntity
            {
                ExpenseID = 14,
                UserID = 5,
                Amount = 1000,
                Date = DateTime.Now,
                CategoryID = 1,
                ReceiptUrl = "url",
                Description = "Test",
                ExpenseType = "UpComing",
                IsActive = true,
                IsRecurring = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "encryptedUser123@example.invalid",
                ModifiedBy = "encryptedUser123@example.invalid"
            }
        };
            _mockUnitOfWork.Setup(u => u.Groups.GetUsersInGroupAsync(groupId)).ReturnsAsync(GroupMembers);
            _mockUnitOfWork.Setup(u => u.Groups.GetExpensesForGroupAsync(groupId)).ReturnsAsync(ExpenseList);
            _mockUnitOfWork.Setup(u => u.Groups.DeleteGroupAsync(groupId)).Returns(Task.CompletedTask);

            var result = await _groupsController.DeleteGroup(groupId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task DeleteGroup_ReturnsFailure_WhenGroupHasUnsettledBalances()
        {
            _mockUnitOfWork.Setup(u => u.Groups.DeleteGroupAsync(99)).Throws(new Exception("Database error"));
            var result = await _groupsController.DeleteGroup(99);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        //[Test]
        //public async Task DeleteGroup_ReturnsServerError_OnException()
        //{
        //    int groupId = 1;
        //    _mockUnitOfWork.Setup(u => u.Groups.GetUserSettlementDetailsAsync(groupId)).Throws(new Exception("Test exception"));

        //    var result = await _groupsController.DeleteGroup(groupId);

        //    Assert.IsNotNull(result);
        //    Assert.IsFalse(result.status);
        //}




    }
}