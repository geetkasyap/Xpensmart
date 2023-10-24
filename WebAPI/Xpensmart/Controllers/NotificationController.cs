using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Xpensmart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : Controller
    {
        public readonly NotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationController> _notificationLogger;

        public NotificationController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<NotificationController> notificationLogger)
        {
            _notificationService = new NotificationService(unitOfWork);
            _configuration = configuration;
            _notificationLogger = notificationLogger;
        }
        // Endpoint to add notifications
        [HttpPost]
        [Route("AddNotifications")]
        public GenericResponse AddNotifications([FromBody] NotificationsEntity notificationsEntities)
        {
            _notificationLogger.LogInformation("Calling Add notification");  // Logging 
            try
            {
                if (notificationsEntities == null) // Checking for null notification data
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:NotificationNull"] };
                }

                notificationsEntities.UsersEntity = null; // Setting user entity to null
                NotificationsEntity ExepenseDetails = _notificationService.AddNotifications(notificationsEntities);
                return new() { status = true, data = _configuration["GenericMessages:Values:AddNotificationsSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:AddNotificationsFail"], error = new Error() { description = ex.Message } };
            }
        }
        // Endpoint to get notification details
        [HttpGet]
        [Route("GetnotificationDetails")]
        public async Task<IActionResult> GetnotificationDetails([FromRoute] int notificationID, [FromQuery] int UserID)
        {
            _notificationLogger.LogInformation("Calling Get notificationDetails");
            try
            {
                if (notificationID == 0) // If notification ID is zero
                {
                    // Fetching all notifications for the user
                    List<NotificationsEntity> notificationsEntities = await Task.FromResult(_notificationService.GetnotificationDetails((int)UserID));
                    return Ok(notificationsEntities);
                }
                else
                {
                    // Fetching specific notification details
                    NotificationsEntity notificationsEntities = await Task.FromResult(_notificationService.GetnotificationDetails((int)notificationID,(int)UserID));
                    return Ok(notificationsEntities);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        // Endpoint to mark a notification as read
        [HttpPut]
        [Route("ReadNotification/{notificationID}")]
        public async Task<GenericResponse> ReadNotification([FromRoute] int notificationID)
        {
            _notificationLogger.LogInformation("calling Read Notification"); // Logging 
            try
            {
                NotificationsEntity notificationsEntities = await Task.FromResult(_notificationService.ReadNotification(notificationID));
                return new() { status = true, data = _configuration["GenericMessages:Values:ReadNotificationSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:ReadNotificationFail"], error = new Error() { description = ex.Message } };
            }
        }
        // Endpoint to delete a notification
        [HttpDelete]
        [Route("DeleteNotification/{notificationID}")]
        public async Task<GenericResponse> DeleteNotification([FromRoute] int notificationID)
        {
            _notificationLogger.LogInformation("calling Delete Notification"); // Logging 
            try
            {
                NotificationsEntity notificationsEntities = await Task.FromResult(_notificationService.DeleteNotification(notificationID));
                return new() { status = true, data = _configuration["GenericMessages:Values:DeleteNotificationSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:DeleteNotificationFail"], error = new Error() { description = ex.Message } };
            }
        }

    }
}
    