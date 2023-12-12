using CoreModels.DTO;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Utils;
using Moq;

namespace Testing.GroupsTests
{
    public class GroupsRepositoryTests:DbContextMock
    {

        [SetUp]
        public void Setup()
        {
            ReinitializeTest();
        }

        [Test]
        public async Task CreateGroupAsync_Success()
        {
            var newGroup = new GroupsEntity
            {
                GroupName = "New Group"
            };

            var result = await _groupsRepository.CreateGroupAsync(newGroup);

            Assert.IsNotNull(result);
            Assert.AreEqual(newGroup.GroupName, result.GroupName);
        }
        [Test]
        public async Task CreateGroupAsync_Fail()
        {
            var newGroup = new GroupsEntity
            {
                GroupName = "New Group"
            };

            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Error creating group: Error creating group: Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _groupsRepository.CreateGroupAsync(newGroup));
            Assert.That(ex.Message, Is.EqualTo("Error creating group: Error creating group: Database error"));
        }

        [Test]
        public async Task AddGroupMembersAsync_Success()
        {
            var members = new List<GroupMembersEntity>
            {
                new GroupMembersEntity { UserID = 1, GroupID = 1, IsActive = true, CreatedOn = DateTime.Now, ModifiedOn = DateTime.Now, CreatedBy = "test", ModifiedBy = "test" },
                new GroupMembersEntity { UserID = 2, GroupID = 1, IsActive = true, CreatedOn = DateTime.Now, ModifiedOn = DateTime.Now, CreatedBy = "test", ModifiedBy = "test" }
            };

            await _groupsRepository.AddGroupMembersAsync(members);

            var result = await _groupsRepository.GetUsersInGroupAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task AddGroupMembersAsync_Fail()
        {
            var members = new List<GroupMembersEntity>
            {
                new GroupMembersEntity { UserID = 1, GroupID = 1, IsActive = true, CreatedOn = DateTime.Now, ModifiedOn = DateTime.Now, CreatedBy = "test", ModifiedBy = "test" },
                new GroupMembersEntity { UserID = 2, GroupID = 1, IsActive = true, CreatedOn = DateTime.Now, ModifiedOn = DateTime.Now, CreatedBy = "test", ModifiedBy = "test" }
            };
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Error adding group members: Error creating group: Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _groupsRepository.AddGroupMembersAsync(members));
            var result = await _groupsRepository.GetUsersInGroupAsync(1);
            Assert.That(ex.Message, Is.EqualTo("Error adding group members: Error creating group: Database error"));
        }

        [Test]
        public async Task GetGroupsForUserAsync_Success()
        {
            var result = await _groupsRepository.GetGroupsForUserAsync(5);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
        [Test]
        public async Task GetGroupsForUserAsync_Fail()
        {
            mockContext.Setup(m => m.Set<GroupMembersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _groupsRepository.GetGroupsForUserAsync(5));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public async Task GetExpensesForGroupAsync_Success()
        {
            var result = await _groupsRepository.GetExpensesForGroupAsync(1);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public async Task GetExpensesForGroupAsync_Fail()
        {
            mockContext.Setup(m => m.Set<GroupExpensesEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _groupsRepository.GetExpensesForGroupAsync(1));

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public async Task AddGroupExpensesAsync_Success()
        {
            var groupExpense = new GroupExpensesEntity
            {
                GroupID = 1,
                ExpenseID = 1,
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "test",
                ModifiedBy = "test"
            };

            await _groupsRepository.AddGroupExpensesAsync(groupExpense);

            var result = await _groupsRepository.GetExpensesForGroupAsync(1);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public async Task AddGroupExpensesAsync_Fail()
        {
            var groupExpense = new GroupExpensesEntity
            {
                GroupID = 1,
                ExpenseID = 1,
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "test",
                ModifiedBy = "test"
            };

            mockContext.Setup(m => m.Set<GroupExpensesEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _groupsRepository.AddGroupExpensesAsync(groupExpense));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public async Task GetUsersInGroupAsync_Success()
        {
            var result = await _groupsRepository.GetUsersInGroupAsync(5);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }


        [Test]
        public async Task GetUsersInGroupAsync_Fail()
        {
            mockContext.Setup(m => m.Set<GroupMembersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _groupsRepository.GetUsersInGroupAsync(5));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public async Task SettleGroupExpensesAsync_Success()
        {
            var userSettlements = new List<UserSettlement>
            {
                new UserSettlement { UserID = 1, TotalOwed = 100 },
                new UserSettlement { UserID = 2, TotalOwed = -100 }
            };

            await _groupsRepository.SettleGroupExpensesAsync(1, userSettlements);
        }

        [Test]
        public async Task SettleGroupExpensesAsync_Fail()
        {
            var userSettlements = new List<UserSettlement>
            {
                new UserSettlement { UserID = 1, TotalOwed = 100 },
                new UserSettlement { UserID = 2, TotalOwed = -100 }
            };

            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _groupsRepository.SettleGroupExpensesAsync(1, userSettlements));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public async Task DeleteGroupAsync_Success()
        {
            await _groupsRepository.DeleteGroupAsync(1);

            var result = await _groupsRepository.GetGroupsForUserAsync(1);
            Assert.IsFalse(result.Any(g => g.GroupID == 1));
        }
        [Test]
        public async Task DeleteGroupAsync_Success_ThrowsException()
        {
            try
            {
                await _groupsRepository.DeleteGroupAsync(10);
                var ex = Assert.ThrowsAsync<System.Exception>(async () =>
                {
                    await _groupsRepository.GetGroupsForUserAsync(10);
                });
            }
            catch(Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Group not found."));

            }
        }
        [Test]
        public async Task DeleteGroupAsync_Fail()
        {
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _groupsRepository.DeleteGroupAsync(1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public async Task AddExpenseAsync_Fail()
        {
            var expenseDetails = new ExpensesEntity
            {
                UserID = 1
            };
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _groupsRepository.AddExpensesAsync(expenseDetails));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


    }
}