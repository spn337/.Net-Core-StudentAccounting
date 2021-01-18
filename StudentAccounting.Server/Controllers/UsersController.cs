using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.Server.Services;
using StudentAccounting.Server.Helpers;
using StudentAccounting.Server.DTOs;
using System.Linq;
using StudentAccounting.Server.Constants;
using System.Transactions;
using System;
using Microsoft.Extensions.Logging;

namespace StudentAccounting.Server.Controllers
{
    [Authorize(Roles.ADMIN)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IEnrollmentService enrollmentService;
        private readonly INotificationService notificationService;
        private readonly IMapper mapper;
        private readonly ILogger<UsersController> logger;

        public UsersController(
            IUserService userService,
            IEnrollmentService enrollmentService,
            INotificationService notificationService,
            IMapper mapper,
            ILogger<UsersController> logger)
        {
            this.userService = userService;
            this.enrollmentService = enrollmentService;
            this.notificationService = notificationService;
            this.mapper = mapper;
            this.logger = logger;
        }


        [HttpGet]
        public IActionResult GetUsers(int pageSize = 10, int currentPage = 1, string searchValue = "",
          string sortOrder = "", string sortName = "")
        {
            if (pageSize > Items.MAX_COUNT)
            {
                return new BadRequestObjectResult(
                    new { Message = $"Max count is {Items.MAX_COUNT} items" });
            }

            var query = userService.Users;

            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(user => user.FirstName.Contains(searchValue)
                                         || user.LastName.Contains(searchValue)
                                         || user.Email.Contains(searchValue)
                                         || user.Age.ToString().Contains(searchValue));
            }

            if (!string.IsNullOrEmpty(sortName))
            {
                query = query.OrderByDynamic(sortName, sortOrder);
            }

            var items = query.Select(user => new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Age,
                RegisteredDate = user.RegisteredDate.ToString(Format.DATETIME_TOSTRING),
                Course = (user.Enrollment != null) ?
                new
                {
                    CourseName = user.Enrollment.Course.Name,
                    StudyDate = user.Enrollment.StudyDate.ToString(Format.DATETIME_TOSTRING)
                } : null
            });

            items = items.Skip((currentPage - 1) * pageSize)
                         .Take(pageSize);

            var totalCount = query.Count();

            return Ok(
                new { Items = items, TotalCount = totalCount });
        }


        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            var user = userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<UserReadDTO>(user));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id)
        {
            var user = userService.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            var enrollment = enrollmentService.GetEnrollmentByUserId(user.Id);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (enrollment != null)
                    {
                        enrollmentService.Unsubscribe(enrollment);
                    }

                    var removeUserResult = userService.RemoveUser(user);
                    if (removeUserResult.Succeeded)
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    scope.Dispose();
                    return BadRequest();
                }
            }
            notificationService.DeleteNotifications(enrollment);

            return NoContent();
        }


        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, UserUpdateDTO userUpdateDTO)
        {
            var user = userService.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            mapper.Map(userUpdateDTO, user);

            var updateUserResult = userService.UpdateUser(user);

            if (!updateUserResult.Succeeded)
            {
                return new BadRequestObjectResult(
                    new { Errors = updateUserResult.GetErrorsFromResult() });
            }

            return NoContent();
        }
    }
}
