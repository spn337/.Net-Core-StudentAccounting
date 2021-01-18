using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.Server.Constants;
using StudentAccounting.Server.Services;
using StudentAccounting.Server.DTOs;
using System;
using System.Linq;
using StudentAccounting.Server.Helpers;

namespace StudentAccounting.Server.Controllers
{
    [Authorize(Roles.STUDENT)]
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService enrollmentService;
        private readonly IAuthService authService;
        private readonly INotificationService notificationService;
        private readonly IMapper mapper;


        public EnrollmentsController(
            IEnrollmentService enrollmentService,
            IAuthService authService,
            INotificationService notificationService,
            IMapper mapper)
        {
            this.enrollmentService = enrollmentService;
            this.authService = authService;
            this.notificationService = notificationService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("Subscribed")]
        public IActionResult GetSubscribedCourses()
        {
            var query = enrollmentService.GetEnrollments(Authorized.UserId);
            var subscribed = query.Select(enrollment => mapper.Map<SubsCourseReadDTO>(enrollment));

            return Ok(
                new { Items = subscribed });
        }

        [HttpGet]
        [Route("UnSubscribed")]
        public IActionResult GetUnsubscribedCourses()
        {
            var query = enrollmentService.GetUnsubscribedCourses(Authorized.UserId);
            var unsubsbscribed = query.Select(course => mapper.Map<CourseReadDTO>(course));

            return Ok(
                new { Items = unsubsbscribed });
        }

        [HttpPost("{courseId}")]
        public IActionResult Subscribe(string courseId, SubsCourseCreateDTO dto)
        {
            if (dto.StudyDate == null)
            {
                return new BadRequestObjectResult(
                     new { Error = "Enter study date" });
            }

            if (!enrollmentService.IsUserExist(Authorized.UserId))
            {
                return NotFound("User is not found");
            }
            if (!enrollmentService.IsCourseExist(courseId))
            {
                return NotFound("Course is not found");
            }
            if (enrollmentService.IsEnrollmentExistByUserId(Authorized.UserId))
            {
                return new BadRequestObjectResult(
                    new { Error = "You can only subscribe to one course" });
            }
            if (enrollmentService.IsEnrollmentExist(Authorized.UserId, courseId))
            {
                return new BadRequestObjectResult(
                    new { Error = "You are alreary subscribe to this course" });
            }

            var studyDate = dto.StudyDate.Date + new TimeSpan(8, 0, 0);

            var enrollment = enrollmentService.Subscribe(Authorized.UserId, courseId, studyDate);

            var isCreateEmails = authService.CreateNotificationEmails(enrollment);
            if (!isCreateEmails)
            {
                return new BadRequestObjectResult(
                    new { Errors = "Sending email is fail!" });
            }

            return Ok();
        }

        [HttpDelete("{courseId}")]
        public IActionResult Unsubscribe(string courseId)
        {
            var enrollment = enrollmentService.GetEnrollmentById(Authorized.UserId, courseId);

            if (enrollment == null)
            {
                return NotFound();
            }

            enrollmentService.Unsubscribe(enrollment);
            notificationService.DeleteNotifications(enrollment);

            return NoContent();
        }

    }
}
