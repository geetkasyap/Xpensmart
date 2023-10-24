using CoreModels.DTO;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IGroupsRepository
    {
        Task<GroupsEntity> CreateGroupAsync(GroupsEntity group);
        Task AddGroupMembersAsync(List<GroupMembersEntity> members);
        Task<List<GroupsEntity>> GetGroupsForUserAsync(int userId);
        Task<List<ExpensesEntity>> GetExpensesForGroupAsync(int groupId);
        Task AddGroupExpensesAsync(GroupExpensesEntity groupExpense);
        Task<List<GroupMember>> GetUsersInGroupAsync(int groupId);
        Task SettleGroupExpensesAsync(int groupID, List<UserSettlement> userSettlements);
        Task DeleteGroupAsync(int groupId);
    }
}
