using CoreModels.DTO;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface ISharedAccountRepository
    {
        SharedAccountsEntity CreateSharedUser(SharedAccountsEntity sharedAccountsEntity);
        SharedAccountsEntity DeleteSharedUser(int? userID);
        List<SharedAccountsEntity> GetSharedUserDetails();
        List<SharedAccountsEntity> GetSharedUserDetails(int userID);
        List<SharedUserWithUserName> GetSharedUserDetailsWithNames(int userID);
        SharedAccountsEntity UpdateSharedUser(int userID1, int userID2);
    }
}
