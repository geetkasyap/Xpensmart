using ApplicationLayer.Interfaces;
using CoreModels.DTO;
using CoreModels.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class GroupsService
    {
        private readonly IUnitOfWork unitOfWork;
        public GroupsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<GroupsEntity> CreateGroupAsync(GroupsEntity group, List<int> userIds)
        {
            try
            {
                var Email = unitOfWork.Users.GetEmailByUserID(userIds[0]);
                var createdGroup = await unitOfWork.Groups.CreateGroupAsync(group);

                var members = userIds.Select(userId => new GroupMembersEntity
                {
                   

                    GroupID = createdGroup.GroupID,
                    UserID = userId,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = Email,
                    ModifiedBy = Email
                }).ToList();

                await unitOfWork.Groups.AddGroupMembersAsync(members);

                return createdGroup;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in service while creating group: {ex.Message}");
            }
        }




        public async Task<List<GroupsEntity>> GetGroupsForUserAsync(int userId)
        {
            try
            {
                return await unitOfWork.Groups.GetGroupsForUserAsync(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in service while fetching groups: {ex.Message}");
            }
        }

        public async Task<List<ExpensesEntity>> GetExpensesForGroupAsync(int groupId)
        {
            try
            {
                return await unitOfWork.Groups.GetExpensesForGroupAsync(groupId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in service while fetching expenses: {ex.Message}");
            }
        }

        public async Task AddGroupExpensesAsync(GroupExpensesEntity groupExpense)
        {
            try
            {
                await unitOfWork.Groups.AddGroupExpensesAsync(groupExpense);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding group expense: {ex.Message}");
            }
        }

        public async Task<List<GroupMember>> GetUsersInGroupAsync(int groupId)
        {
            try
            {
                return await unitOfWork.Groups.GetUsersInGroupAsync(groupId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching users in group: {ex.Message}");
            }
        }

        public async Task SettleGroupExpensesAsync(int groupID, List<UserSettlement> userSettlements)
        {
            try
            {
                await unitOfWork.Groups.SettleGroupExpensesAsync(groupID,userSettlements);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching users in group: {ex.Message}");
            }
        }
        //settle expenses owed money details 
        public async Task<List<UserBalance>> GetUserSettlementDetailsAsync(int groupId)
        {
            try
            {
                var users = await unitOfWork.Groups.GetUsersInGroupAsync(groupId);
                var expenses = await unitOfWork.Groups.GetExpensesForGroupAsync(groupId);

                decimal totalSettleAmountDifference = 0;

                var userSettlements = users.Select(user =>
                {
                    var userExpenses = expenses.Where(e => e.UserID == user.UserID);
                    var totalSpent = Math.Round(userExpenses.Where(e => e.ExpenseType != "Credited").Sum(e => e.Amount) -
                                                userExpenses.Where(e => e.ExpenseType == "Credited").Sum(e => e.Amount), 2, MidpointRounding.AwayFromZero); 

                    var totalMembers = users.Count;
                    var averageShare = Math.Round(((expenses.Where(e => e.ExpenseType != "Credited").Sum(e => e.Amount) -
                                                    expenses.Where(e => e.ExpenseType == "Credited").Sum(e => e.Amount)) / totalMembers), 2, MidpointRounding.AwayFromZero);

                    var totalOwed = Math.Round((averageShare - totalSpent), 2, MidpointRounding.AwayFromZero);

                    totalSettleAmountDifference += totalOwed; //adding the residuals to a user
                     
                    if (totalSettleAmountDifference > 0 && totalSettleAmountDifference < 1)
                    {
                        totalOwed += totalSettleAmountDifference;
                        totalSettleAmountDifference = 0;
                    }
                    else if (totalSettleAmountDifference > -1 && totalSettleAmountDifference < 0)
                    {
                        totalOwed -= totalSettleAmountDifference;
                        totalSettleAmountDifference = 0;
                    }

                    return new UserBalance
                    {
                        UserID = user.UserID,
                        UserName = user.UserName,
                        NetBalance = totalOwed
                    };
                }).ToList();

                return userSettlements;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching user settlement details: {ex.Message}");
            }
        }

        public async Task DeleteGroupAsync(int groupId)
        {
            try
            {
                await unitOfWork.Groups.DeleteGroupAsync(groupId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting group: {ex.Message}");
            }
        }
    }
}
