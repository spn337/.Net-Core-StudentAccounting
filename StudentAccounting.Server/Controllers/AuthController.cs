
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.Server.DTOs;
using StudentAccounting.Server.Services;
using StudentAccounting.Server.Helpers;
using StudentAccounting.Server.Constants;
using StudentAccounting.Server.Domain.Entities;

namespace StudentAccounting.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public AuthController(IAuthService authService, IUserService userService, IMapper mapper)
        {
            this.authService = authService;
            this.userService = userService;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        [Route("Profile")]
        public IActionResult GetUserProfile()
        {
            var user = userService.GetUserById(Authorized.UserId);
            if (user == null)
            {
                return Unauthorized();
            }

            var roles = authService.GetUserRoles(user);
            return Ok(
                 new
                 {
                     User = mapper.Map<UserProfileReadDTO>(user),
                     Roles = roles
                 }
           );
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginDTO loginDTO)
        {

            var user = userService.GetUserByEmail(loginDTO.Email);
            var isPasswordValid = authService.CheckPassword(user, loginDTO.Password);

            if (user == null || !isPasswordValid)
            {
                return new BadRequestObjectResult(
                    new { Error = "Wrong data. Try again" });
            }

            if (!user.EmailConfirmed)
            {
                return new BadRequestObjectResult(
                    new { Error = "Email is not confirmed yet" });
            }

            var result = authService.Login(loginDTO.Email, loginDTO.Password, loginDTO.RememberMe);

            if (result.Succeeded)
            {
                var roles = authService.GetUserRoles(user);
                var token = authService.GenerateJwtToken(user, roles);

                return Ok(new
                {
                    User = mapper.Map<UserProfileReadDTO>(user),
                    Roles = roles,
                    Token = token
                });
            }

            return new BadRequestObjectResult(
                new { Error = "Login failed!" });
        }


        [HttpPost]
        [Route("Registration")]
        public IActionResult Registration(UserCreateDTO userCreateDTO)
        {
            var user = mapper.Map<DbUser>(userCreateDTO);
            var password = userCreateDTO.Password;

            var createUserResult = userService.CreateUser(user, password);

            if (!createUserResult.Succeeded)
            {
                return new BadRequestObjectResult(
                  new { Errors = createUserResult.GetErrorsFromResult() });
            }

            var isSendEmailSuccessed = authService.SendConfirmAccountEmail(user);
            if (!isSendEmailSuccessed)
            {
                return new BadRequestObjectResult(
                    new { Errors = "Sending email is fail!" });
            }
            return Ok(mapper.Map<UserReadDTO>(user));
        }

        [HttpDelete]
        [Route("Logout")]
        public IActionResult Logout()
        {
            authService.Logout();
            return NoContent();
        }


        [HttpGet]
        [Route("ConfirmEmail")]
        public IActionResult AfterConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return new BadRequestObjectResult(
                       new { Error = $"The User ID {userId} or token {token} are invalid" });
            }

            var user = userService.GetUserById(userId);
            if (user == null)
            {
                return new BadRequestObjectResult(
                       new { Error = $"The User ID {userId} is invalid" });

            }

            if (user.EmailConfirmed)
            {
                return new BadRequestObjectResult(
                       new { Error = $"Email already confirmed" });
            }

            var result = authService.ConfirmEmail(user, token);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return new BadRequestObjectResult(
                    new { Error = $"Email cannot be confirmed or token's lificycle was over" });
            }
        }
    }
}
