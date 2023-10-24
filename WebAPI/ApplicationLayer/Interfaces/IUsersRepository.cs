using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreModels.Entities;

namespace ApplicationLayer.Interfaces
{
    public interface IUsersRepository
    {
        UsersEntity CreateUser(UsersEntity usersEntities);
        List<UsersEntity> GetUserDetails();
        public UsersEntity GetUserDetails(string userName);
        UsersEntity UpdateUser(string? userName, string password,string NewPassword);
        UsersEntity DeleteUser(string? userName);
        UsersEntity CheckUserName(string? userName);
        Task<UsersEntity> GetUserByUserNameAsync(string userName);
        UsersEntity LoginCheck(string? email, string? password);
        string GetUserNamesByUserID(int userID);
        string GetEmailByUserID(int userID);
        string[] GetUserAllNames();
        UsersEntity CheckEmail(string email);
        string Encryption(string password);
        string Decryption(string hashedPassword);
        string ForgotPassword(string email);
    }
}
