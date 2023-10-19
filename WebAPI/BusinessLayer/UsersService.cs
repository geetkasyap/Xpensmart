using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class UsersService
    {
        private readonly IUnitOfWork unitOfWork;
        public UsersService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public UsersEntity CreateUser(UsersEntity usersEntities)
        {
            return unitOfWork.Users.CreateUser(usersEntities);
        }

        public List<UsersEntity> GetUserDetails()
        {
            return unitOfWork.Users.GetUserDetails();
        }

        public UsersEntity GetUserDetails(string userName)
        {
            return unitOfWork.Users.GetUserDetails(userName);
        }

        public UsersEntity UpdateUser(string? userName, string password,string NewPassword)
        {
            return unitOfWork.Users.UpdateUser(userName, password, NewPassword);
        }
        public UsersEntity DeleteUser(string? userName)
        {
            return unitOfWork.Users.DeleteUser(userName);
        }

        public UsersEntity CheckUserName(string? userName)
        {
            return unitOfWork.Users.CheckUserName(userName);
        }

        public UsersEntity LoginCheck(string? email, string? password)
        {
            return unitOfWork.Users.LoginCheck(email, password);
        }

        public string GetUserNamesByUserID(int userID)
        {
            return unitOfWork.Users.GetUserNamesByUserID(userID);
        }

        public string[] GetUserAllNames()
        {
            return unitOfWork.Users.GetUserAllNames();
        }

        public UsersEntity CheckEmail(string email)
        {
            return unitOfWork.Users.CheckEmail(email);
        }

        public string Encryption(string password)
        {
            return unitOfWork.Users.Encryption(password);
        }

        public string Decryption(string HashedPassword)
        {
            return unitOfWork.Users.Decryption(HashedPassword);
        }

        public string ForgotPassword(string email)
        {
            return unitOfWork.Users.ForgotPassword(email);
        }
    }
}
