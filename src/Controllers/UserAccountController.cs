using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using theforum.BusinessLogic;
using theforum.Dto;

namespace theforum.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserAccountController : ControllerBase
{
    private readonly UserAccountService _userAccountService;

    public UserAccountController(UserAccountService userAccountService) => 
        _userAccountService = userAccountService;

    [HttpPost]
    [Route("Register")]
    public async Task Register(UserRegistrationSubmissionDto userSubmission) => 
        await _userAccountService.CreateUser(userSubmission.EmailAddress,
            userSubmission.Username,
            userSubmission.Password,
            userSubmission.PasswordConfirm);

    [HttpPost]
    [Route("RequestPasswordReset")]
    public async Task<IActionResult> RequestPasswordReset(RequestPasswordResetDto requestPasswordResetDto) =>
        await _userAccountService.RequestPasswordReset(requestPasswordResetDto.EmailAddress) 
            ? Ok() 
            : NotFound();

    [HttpPost]
    [Route("VerifyPasswordResetEmail")]
    public async Task<bool> VerifyPasswordResetEmail(RequestPasswordResetDto requestPasswordResetDto) => 
        await _userAccountService.VerifyPasswordResetEmail(requestPasswordResetDto.EmailAddress);

    [HttpPost]
    [Route("UpdatePassword")]
    public async Task UpdatePassword (UpdatePasswordDto updatePasswordDto) =>
        await _userAccountService.UpdatePassword(
            updatePasswordDto.EmailAddress,
            updatePasswordDto.AuthenticationCode,
            updatePasswordDto.Password,
            updatePasswordDto.PasswordConfirm);

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(LoginCredentialsDto loginCredentials)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var userId = await _userAccountService.ValidateUser(loginCredentials.Username, loginCredentials.Password);
        if (userId <= 0)
        {
            return Unauthorized();
        }
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, loginCredentials.Username),
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(60)
        };
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
        return Ok();
    }

    [HttpPost]
    [Route("Logout")]
    public async Task Logout() => 
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}