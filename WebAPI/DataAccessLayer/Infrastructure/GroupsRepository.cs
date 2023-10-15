using ApplicationLayer.Interfaces;
using CoreModels.DTO;
using CoreModels.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure
{
    public class GroupsRepository : IGroupsRepository
    {
        private readonly ApplicationDbContext _context;
        public GroupsRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        // Asynchronously creates a new group and saves it to the database
        public async Task<GroupsEntity> CreateGroupAsync(GroupsEntity group)
        {
            try
            {
                _context.Set<GroupsEntity>().Add(group); // Add the new group to the context
                await _context.SaveChangesAsync(); // Save changes to the database
                return group; 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Asynchronously adds multiple members to a group
        public async Task AddGroupMembersAsync(List<GroupMembersEntity> members)
        {
            try
            {
                _context.Set<GroupMembersEntity>().AddRange(members);// Add members to the context
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Asynchronously retrieves all groups for a specific user
        public async Task<List<GroupsEntity>> GetGroupsForUserAsync(int userId)
        {
            try
            {
                return _context.Set<GroupMembersEntity>()
                    .Where(gm => gm.UserID == userId && gm.IsActive)
                    .Select(gm => gm.GroupsEntity).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Asynchronously retrieves all expenses for a specific group
        public async Task<List<ExpensesEntity>> GetExpensesForGroupAsync(int groupId)
        {
            try
            {
                return _context.Set<GroupExpensesEntity>()
            .Where(ge => ge.GroupID == groupId && ge.IsActive)
            .Select(ge => ge.ExpensesEntity)
            .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Asynchronously adds a new expense to a group
        public async Task AddGroupExpensesAsync(GroupExpensesEntity groupExpense)
        {
            try
            {
                _context.Set<GroupExpensesEntity>().Add(groupExpense);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Asynchronously retrieves all users in a specific group
        public async Task<List<GroupMember>> GetUsersInGroupAsync(int groupId)
        {
            try
            {
                return _context.Set<GroupMembersEntity>()
                    .Where(gm => gm.GroupID == groupId && gm.IsActive)
                    .Join(_context.Set<UsersEntity>(),
                        gm => gm.UserID,
                        user => user.UserID,
                        (gm, user) => new GroupMember // Create a new GroupMember object
                        {
                            UserID = user.UserID,
                            UserName = user.UserName,
                            Email = user.Email
                        })
                    .ToList(); // Join with users to get user details
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Asynchronously settles expenses among group members based on their balances
        public async Task SettleGroupExpensesAsync(int groupID, List<UserSettlement> userSettlements)
        {
            try
            {
                var userBalances = userSettlements
                    .Select(user => new UserBalance
                    {
                        UserID = user.UserID,
                        UserName = _context.Set<UsersEntity>().Where(a => a.UserID == user.UserID && a.IsActive).Select(a=>a.UserName).FirstOrDefault(),
                        NetBalance = user.TotalOwed
                    })
                    .ToList();
                // Separate debtors and creditors based on their balances
                var debtors = userBalances.Where(u => u.NetBalance > 0)
                                          .OrderByDescending(u => u.NetBalance)
                                          .ToList();
                var creditors = userBalances.Where(u => u.NetBalance < 0)
                                            .OrderBy(u => u.NetBalance)
                                            .ToList();
                // Settle debts between debtors and creditors
                while (debtors.Any() && creditors.Any())
                {
                    var debtor = debtors.First(); // Get the first debtor
                    var creditor = creditors.First(); // Get the first creditor
                    var amountToSettle = Math.Min(debtor.NetBalance, Math.Abs(creditor.NetBalance)); // Determine the amount to settle

                    debtor.NetBalance -= amountToSettle; // Update debtor's balance
                    creditor.NetBalance += amountToSettle; // Update creditor's balance

                    string debtorUsername = debtor.UserName; // Get debtor user entity
                    string creditorUsername = creditor.UserName; // Get creditor user entity
                    UsersEntity debtorUser = _context.Set<UsersEntity>().Where(a => a.UserName == debtorUsername && a.IsActive).FirstOrDefault(); // Get debtor user entity
                    UsersEntity creditorUser = _context.Set<UsersEntity>().Where(a => a.UserName == creditorUsername && a.IsActive).FirstOrDefault(); // Get creditor user entity

                    // Create settlement expenses for both debtor and creditor
                    await CreateSettlementExpense(debtorUser, creditorUser, amountToSettle, "Business", groupID);
                    await CreateSettlementExpense(creditorUser, debtorUser, amountToSettle, "Credited", groupID);

                    // Remove users from lists if their balances are settled
                    if (debtor.NetBalance == 0) debtors.Remove(debtor);
                    if (creditor.NetBalance == 0) creditors.Remove(creditor);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Asynchronously creates a settlement expense record
        private async Task CreateSettlementExpense(UsersEntity fromUser, UsersEntity toUser,decimal amount, string expenseType, int groupID)
        {
            var expense = new ExpensesEntity
            {
                UserID = fromUser.UserID,
                Amount = amount,
                Date = DateTime.Now,
                CategoryID = 12, // setting as misc
                ReceiptUrl = string.Empty,
                Description = expenseType == "Business"
                              ? $"Paid to {toUser.FullName}"
                              : $"Received from {fromUser.FullName}",
                ExpenseType = expenseType,
                IsActive = true,
                IsRecurring = false,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = fromUser.Email,
                ModifiedBy = fromUser.Email
            };

            var addedExpense = await AddExpensesAsync(expense); // Add the expense to the database

            var groupExpense = new GroupExpensesEntity
            {
                GroupID = groupID,
                ExpenseID = addedExpense.ExpenseID,
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = fromUser.Email,
                ModifiedBy = fromUser.Email
            };

            await _context.Set<GroupExpensesEntity>().AddAsync(groupExpense);
            await _context.SaveChangesAsync();
        }
        // Asynchronously adds a new expense entity to the database
        public async Task<ExpensesEntity> AddExpensesAsync(ExpensesEntity expensesEntities)
       {
            try
            {
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserID == expensesEntities.UserID && a.IsActive).FirstOrDefault();
                ExpensesEntity newExpenses = new ExpensesEntity
                {
                    UserID = expensesEntities.UserID,
                    Amount = expensesEntities.Amount,
                    Date = expensesEntities.Date,
                    CategoryID = expensesEntities.CategoryID,
                    ReceiptUrl = expensesEntities.ReceiptUrl,
                    Description = expensesEntities.Description,
                    ExpenseType = expensesEntities.ExpenseType,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = loginEntity.Email,
                    ModifiedBy = loginEntity.Email,
                };
                _context.Set<ExpensesEntity>().Add(newExpenses);
                await _context.SaveChangesAsync();
                return newExpenses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
       }
        // Asynchronously deletes a group by marking it as inactive
        public async Task DeleteGroupAsync(int groupId)
        {
            try
            {
                // Soft delete the group by marking it as inactive
                var group = _context.Set<GroupsEntity>()
                                          .FirstOrDefault(g => g.GroupID == groupId);

                if (group == null)
                {
                    throw new Exception("Group not found.");
                }

                group.IsActive = false;
                group.ModifiedOn = DateTime.Now;

                // Also mark all related group members as inactive
                var groupMembers = _context.Set<GroupMembersEntity>()
                                                 .Where(gm => gm.GroupID == groupId)
                                                 .ToList();

                foreach (var member in groupMembers)
                {
                    member.IsActive = false;
                    member.ModifiedOn = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
