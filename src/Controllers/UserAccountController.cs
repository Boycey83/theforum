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

    public UserAccountController(UserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
    }

    [HttpPost]
    [Route("Register")]
    public void Register(UserRegistrationSubmissionDto userSubmission)
    {
        _userAccountService.CreateUser(userSubmission.EmailAddress,
            userSubmission.Username,
            userSubmission.Password,
            userSubmission.PasswordConfirm);
    }

    [HttpPost]
    [Route("RequestPasswordReset")]
    public IActionResult RequestPasswordReset(RequestPasswordResetDto requestPasswordResetDto)
    {
        return _userAccountService.RequestPasswordReset(requestPasswordResetDto.EmailAddress) 
            ? Ok() 
            : NotFound();
    }

    [HttpPost]
    [Route("VerifyPasswordResetEmail")]
    public bool VerifyPasswordResetEmail(RequestPasswordResetDto requestPasswordResetDto)
    {
        return _userAccountService.VerifyPasswordResetEmail(requestPasswordResetDto.EmailAddress);
    }

    [HttpPost]
    [Route("UpdatePassword")]
    public void UpdatePassword (UpdatePasswordDto updatePasswordDto)
    {
        _userAccountService.UpdatePassword(
            updatePasswordDto.EmailAddress,
            updatePasswordDto.AuthenticationCode,
            updatePasswordDto.Password,
            updatePasswordDto.PasswordConfirm);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(LoginCredentialsDto loginCredentials)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var userId = _userAccountService.ValidateUser(loginCredentials.Username, loginCredentials.Password);
        if (userId <= 0)
        {
            return Unauthorized();
        }
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginCredentials.Username),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
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
    public async Task Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }   
}