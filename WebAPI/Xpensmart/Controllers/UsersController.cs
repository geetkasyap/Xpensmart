using Microsoft.AspNetCore.Mvc;
using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using CoreModels.DTO;

namespace Xpensmart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        public readonly UsersService _usersService;
        public readonly SharedAccountService _sharedAccountService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsersController> _usersLogger;
        public UsersController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<UsersController> UsersLogger)
        {
            _usersService = new UsersService(unitOfWork);
            _sharedAccountService = new SharedAccountService(unitOfWork);
            _configuration = configuration;
            _usersLogger = UsersLogger;
        }

        //Create User
        [HttpPost]
        [Route("CreateUser")]
        public async Task<GenericResponse> CreateUser([FromBody] UsersEntity usersEntities)
        {
            _usersLogger.LogInformation("Calling Add User"); //Logging create user 
            try
            {
                if (usersEntities == null)
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:AddUserEmpty"] };
                }
                //Assigning Icollections as null intially
                usersEntities.BudgetsEntities = null;
                usersEntities.ExpenseProjectionEntities = null;
                usersEntities.ExpensesEntities = null;
                usersEntities.NotificationsEntities = null;
                usersEntities.ReportsEntities = null;


                UsersEntity UserDetails =await  Task.FromResult(_usersService.CreateUser(usersEntities));
                return new() { status = true, data = _configuration["GenericMessages:Values:AddUserSuccess"] };
            }
            catch (Exception ex)
            {
                _usersLogger.LogError(ex, "An error occurred while fetching user details.");
                return new() { status = false, data = _configuration["GenericMessages:Values:AddUserFail"] ,error=new Error() { description = ex.Message } };
            }
        }

        //Get User Entity by UserName
        [HttpGet]
        [Route("GetUserDetails/{UserName}")]
        public async Task<IActionResult> GetUserDetails([FromRoute] string? UserName)
        {
            _usersLogger.LogInformation("Calling Get UserDetails"); //Logging GetUserDetails
            try
            {
                if (string.IsNullOrEmpty(UserName)) //if user name is empty
                {
                    return StatusCode(403, new { Error = "User Name cannot be null or empty." });
                }

                if (UserName == "All") //if username is entered as all
                {
                    List<UsersEntity> loginEntities = await Task.FromResult(_usersService.GetUserDetails()); //list of all user entities will be returned
                    return Ok(loginEntities);
                }
                else if (UserName == "AllNames") //if username is entered as AllNames
                {
                    string[] loginEntities = await Task.FromResult(_usersService.GetUserAllNames()); //list of all usernames will be returned
                    return Ok(loginEntities);
                }
                else
                {
                    UsersEntity loginEntity = await Task.FromResult(_usersService.GetUserDetails(UserName)); //particular user entity will be returned 
                    if (loginEntity == null)
                    {
                        return StatusCode(403, new { Error = "User  not found." });
                    }
                    return Ok(loginEntity);
                }
            }
            catch (Exception ex)
            {
                _usersLogger.LogError(ex, "An error occurred while fetching user details.");
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        //Returning username by inputting userID
        [HttpGet]
        [Route("GetUserNamesByUserID/{UserID}")]
        public async Task<IActionResult> GetUserNamesByUserID([FromRoute] int? UserID)
        {
            _usersLogger.LogInformation("Calling Get UserDetails"); //logging get user names by userid
            try
            {
                if (UserID == 0)
                {
                    return StatusCode(403, new { Error = "User ID cannot be null." });
                }

                string userName = await Task.FromResult(_usersService.GetUserNamesByUserID((int)UserID)); //return as string

                return Ok(new { UserName = userName });
            }
            catch (Exception ex)
            {
                _usersLogger.LogError(ex, "An error occurred while getting user names.");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        //update user entity
        [HttpPut]
        [Route("UpdateUser/{UserName}")]
        public async Task<GenericResponse> UpdateUser([FromRoute] string? UserName, [FromQuery] string Password, [FromQuery] string NewPassword)
        {
            _usersLogger.LogInformation("Calling Update User"); //logging update user
            try
            {
                UsersEntity loginEntities = await Task.FromResult(_usersService.UpdateUser(UserName, Password, NewPassword)); //change password 
                if(loginEntities == null)
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:UpdateUserDetailsFail"] };
                }
                else
                {
                    return new() { status = true, data = _configuration["GenericMessages:Values:UpdateUserDetailsSuccess"] };
                }
            }
            catch (Exception ex)
            {
                _usersLogger.LogError(ex, "An error occurred while fetching user details.");
                return new() { status = false, data = _configuration["GenericMessages:Values:UpdateUserDetailsFail"], error = new Error() { description = ex.Message } };
            }
        }

        //soft delete user entity by username 
        [HttpDelete]
        [Route("DeleteUser/{UserName}")]
        public async Task<GenericResponse> DeleteUser([FromRoute] string? UserName)
        {
            _usersLogger.LogInformation("Calling Delete User"); //logging delete user
            try
            {
                UsersEntity UsersEntity = await Task.FromResult(_usersService.DeleteUser(UserName));
                return new() { status = true, data = _configuration["GenericMessages:Values:DeleteUserSuccess"] };
            }
            catch (Exception ex)
            {
                _usersLogger.LogError(ex, "An error occurred while fetching user details.");
                return new() { status = false, data = _configuration["GenericMessages:Values:DeleteUserFail"], error = new Error() { description = ex.Message } };
            }

        }

        //checking username which have to be unique
        [HttpGet]
        [Route("CheckUserName/{UserName}")]
        public async Task<GenericResponse> CheckUserName([FromRoute] string? UserName)
        {
            _usersLogger.LogInformation("Calling CheckUserName"); //logging checkusername
            try
            {
                if (string.IsNullOrEmpty(UserName)) //if null or empty was entered
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:UserNameNull"] };
                }
                UsersEntity loginEntities = await Task.FromResult(_usersService.CheckUserName((string)UserName));
                if (loginEntities == null)
                {
                    return new() { status = true, data = _configuration["GenericMessages:Values:AvailableUserNameSuccess"] };
                }
                else
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:AvailableUserNameFail"] };
                }
            }
            catch (Exception ex)
            {
                _usersLogger.LogError(ex, "Error checking username availability");
                return new() { status = false, data = _configuration["GenericMessages:Values:AvailableUserNameFail"], error = new Error() { description = ex.Message } };
            }
        }

        //check Email that need to be unique
        [HttpGet]
        [Route("CheckEmail/{Email}")]
        public async Task<GenericResponse> CheckEmail([FromRoute] string? Email)
        {
            _usersLogger.LogInformation("Calling CheckEmail"); //logging check Email
            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:AvailableEmailFail"] };
                }
                UsersEntity loginEntities = await Task.FromResult(_usersService.CheckEmail((string)Email));
                if (loginEntities == null)
                {
                    return new() { status = true, data = _configuration["GenericMessages:Values:AvailableEmailSuccess"] };
                }
                else
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:AvailableEmailFail"] };
                }
            }
            catch (Exception ex)
            {
                _usersLogger.LogError(ex, "Error checking Email availability");
                return new() { status = false, data = _configuration["GenericMessages:Values:AvailableEmailFail"], error = new Error() { description = ex.Message } };
            }
        }

        // create shared users
        [HttpPost]
        [Route("CreateSharedUser")]
        public GenericResponse CreateSharedUser([FromBody] SharedAccountsEntity sharedAccountsEntity)
        {
            _usersLogger.LogInformation("Calling CreateSharedUser");
            try
            {
                if (sharedAccountsEntity == null)
                {

                    return new() { status = false, data = _configuration["GenericMessages:Values:CreateSharedUserNull"] };
                }
                SharedAccountsEntity UserDetails = _sharedAccountService.CreateSharedUser(sharedAccountsEntity); //create shared user entity 
                return new() { status = true, data = _configuration["GenericMessages:Values:CreateSharedUserSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:CreateSharedUserFail"], error = new Error() { description = ex.Message } };
            }
        }

        //Return shared user entities
        [HttpGet]
        [Route("GetSharedUserDetails/{UserID}")]
        public async Task<IActionResult> GetSharedUserDetails([FromRoute] int UserID)
        {
            _usersLogger.LogInformation("Calling GetSharedUserDetails");
            try
            {
                if (UserID == 0)  //if userID is zero then return list of all the users in shared entity
                {
                    List<SharedAccountsEntity> sharedAccountsEntity = await Task.FromResult(_sharedAccountService.GetSharedUserDetails());
                    return Ok(sharedAccountsEntity);
                }
                else //else return particular user 
                {
                    List<SharedAccountsEntity> sharedAccountsEntity = await Task.FromResult(_sharedAccountService.GetSharedUserDetails((int)UserID));
                    return Ok(sharedAccountsEntity);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        //update users in shared user entity
        [HttpPut]
        [Route("UpdateSharedUser")]
        public async Task<GenericResponse> UpdateSharedUser([FromQuery] int UserID1, [FromQuery] int UserID2)
        {
            _usersLogger.LogInformation("Calling UpdateSharedUser"); //logging update shared user
            try
            {
                if (UserID1 != 0 && UserID2 != 0)
                {
                    SharedAccountsEntity sharedAccountsEntity = await Task.FromResult(_sharedAccountService.UpdateSharedUser(UserID1, UserID2));
                    return new() { status = true, data = _configuration["GenericMessages:Values:UpdateSharedUserSuccess"] };
                }
                else
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:UpdateSharedUserFail"] };
                }

            }
            catch (Exception ex)
            {
                _usersLogger.LogError(ex, "An error occurred while fetching user details.");
                return new() { status = false, data = _configuration["GenericMessages:Values:UpdateSharedUserFail"], error = new Error() { description = ex.Message } };
            }
        }

        //soft delete the shared user entity
        [HttpDelete]
        [Route("DeleteSharedUser")]
        public async Task<GenericResponse> DeleteSharedUser([FromRoute] int? UserID)
        {
            _usersLogger.LogInformation("Calling Delete SharedUser"); //logging delete shared user
            try
            {
                if (UserID == null)
                {
                    SharedAccountsEntity sharedAccountsEntity = await Task.FromResult(_sharedAccountService.DeleteSharedUser(UserID));
                    return new() { status = false, data = _configuration["GenericMessages:Values:DeleteSharedUserFail"] };
                }
                else
                {
                    SharedAccountsEntity sharedAccountsEntity = await Task.FromResult(_sharedAccountService.DeleteSharedUser(UserID));
                    return new() { status = true, data = _configuration["GenericMessages:Values:DeleteSharedUserSuccess"] };
                }

            }
            catch (Exception ex)
            {
                _usersLogger.LogError(ex, "An error occurred while fetching user details.");
                return new() { status = false, data = _configuration["GenericMessages:Values:DeleteSharedUserFail"], error = new Error() { description = ex.Message } };
            }
        }

        //login aunthetication
        [HttpGet]
        [Route("LoginCheck/{Email}")]
        public async Task<IActionResult> LoginCheck([FromRoute] string? Email, [FromQuery] string? Password)
        {
            _usersLogger.LogInformation("Calling LoginCheck"); //logging login users

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                return BadRequest("Email and Password are required.");
            }
            try
            {
                UsersEntity loginEntities = await Task.FromResult(_usersService.LoginCheck(Email, Password));
                if (loginEntities == null)
                {
                    return Unauthorized("Invalid credentials.");
                }
                return Ok(loginEntities);

            }
            catch (Exception ex)
            {
                _usersLogger.LogError($"Error in LoginCheck: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        //Forgot Password
        [HttpGet]
        [Route("ForgotPassoword/{Email}")]
        public async Task<GenericResponse> ForgotPassoword([FromRoute] string? Email)
        {
            _usersLogger.LogInformation("Calling ForgotPassoword"); //logging forgot password

            if (string.IsNullOrEmpty(Email))
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:EmailRequired"] };            
            }
            try
            {
                string Password = await Task.FromResult(_usersService.ForgotPassword(Email));
                if (Password == null)
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:InvalidEmail"] };
                }
                return new() { status = true, data = Password };
            }
            catch (Exception ex)
            {
                _usersLogger.LogError($"Error in ForgotPassword: {ex.Message}");
                return new() { status = false, data = _configuration["GenericMessages:Values:ForgotPasswordFail"], error = new Error() { description = ex.Message } };
            }
        }

        // list of shared user entities
        [HttpGet]
        [Route("GetSharedUserDetailsWithNames/{UserID}")]
        public async Task<IActionResult> GetSharedUserDetailsWithNames([FromRoute] int UserID)
        {
            _usersLogger.LogInformation("Calling GetSharedUserDetailsWithNames"); //logging Get Shared User Details With Names
            try
            {
                if (UserID == 0)
                {
                    List<SharedUserWithUserName> sharedAccountsWithNames = await Task.FromResult(_sharedAccountService.GetSharedUserDetailsWithNames(UserID));
                    return Ok(sharedAccountsWithNames);
                }
                else
                {
                    List<SharedUserWithUserName> sharedAccountsWithNames = await Task.FromResult(_sharedAccountService.GetSharedUserDetailsWithNames(UserID));
                    return Ok(sharedAccountsWithNames);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        //password encryption
        [HttpGet]
        [Route("EncryptionOfPassword/{Password}")]
        public async Task<IActionResult> Encryption([FromRoute] string Password)
        {
            _usersLogger.LogInformation("Calling EncryptionOfPassword"); //logging Encryption Of Password
            try
            {
                string HashCode = await Task.FromResult(_usersService.Encryption(Password));
                return Ok(HashCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        //password decryption
        [HttpGet]
        [Route("DecryptionOfPassword/{HashedPassword}")]
        public async Task<IActionResult> Decryption([FromRoute] string HashedPassword)
        {
            _usersLogger.LogInformation("Calling DecryptionOfPassword"); //logging Decryption Of Password
            try
            {
                string HashCode = await Task.FromResult(_usersService.Decryption(HashedPassword));
                return Ok(HashCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    }
}
