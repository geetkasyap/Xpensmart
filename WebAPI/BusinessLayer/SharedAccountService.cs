using ApplicationLayer.Interfaces;
using CoreModels.DTO;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class SharedAccountService
    {
        private readonly IUnitOfWork unitOfWork;
        public SharedAccountService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public SharedAccountsEntity CreateSharedUser(SharedAccountsEntity sharedAccountsEntity)
        {
            return unitOfWork.sharedAccount.CreateSharedUser(sharedAccountsEntity);
        }

        public SharedAccountsEntity DeleteSharedUser(int? userID)
        {
            return unitOfWork.sharedAccount.DeleteSharedUser(userID);
        }

        public List<SharedAccountsEntity> GetSharedUserDetails()
        {
            return unitOfWork.sharedAccount.GetSharedUserDetails();
        }

        public List<SharedAccountsEntity> GetSharedUserDetails(int userID)
        {
            return unitOfWork.sharedAccount.GetSharedUserDetails(userID);
        }

        public List<SharedUserWithUserName> GetSharedUserDetailsWithNames(int userID)
        {
            return unitOfWork.sharedAccount.GetSharedUserDetailsWithNames(userID);
        }

        public SharedAccountsEntity UpdateSharedUser(int userID1, int userID2)
        {
            return unitOfWork.sharedAccount.UpdateSharedUser(userID1, userID2);
        }
    }
}
