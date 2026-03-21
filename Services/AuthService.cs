using System.Security.Cryptography;
using System.Text;
using EduBridge.Abstractions;
using EduBridge.Abstractions.Consts;
using EduBridge.Authentication;
using EduBridge.Contracts.Authentication;
using EduBridge.Entities;
using EduBridge.Errors;
using EduBridge.Helpers;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace EduBridge.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly int _refreshTokenExpiryDays = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(
        string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var result = await _signInManager.PasswordSignInAsync(user, password, false, true);

        if (result.Succeeded)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles);

            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(x => x.IsActive);

            string refreshToken;
            DateTime refreshTokenExpiry;

            if (activeRefreshToken != null)
            {
                refreshToken = activeRefreshToken.Token;
                refreshTokenExpiry = activeRefreshToken.ExpiresOn;
            }
            else
            {
                refreshToken = GenerateRefreshToken();
                refreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresOn = refreshTokenExpiry
                });

                await _userManager.UpdateAsync(user);
            }

            var response = new AuthResponse(token, expiresIn, refreshToken, refreshTokenExpiry);

            return Result.Success(response);
        }

        var error = result.IsNotAllowed
            ? UserErrors.EmailNotConfirmed
            : result.IsLockedOut
                ? UserErrors.LockedUser
                : UserErrors.InvalidCredentials;

        return Result.Failure<AuthResponse>(error);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(
        string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);


        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var userRoles = await _userManager.GetRolesAsync(user);

        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(
        string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result> RegisterAsync(
        RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailIsExists = await _userManager.Users
            .AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailIsExists)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code: {code}", code);

            await SendConfirmationEmail(user, code, request.Role);

            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            var roleToAssign = string.IsNullOrWhiteSpace(request.Role)
                ? DefaultRoles.Student
                : request.Role;

            await _userManager.AddToRoleAsync(user, roleToAssign);
            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }


    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation code: {code}", code);

        await SendConfirmationEmail(user, code);

        return Result.Success();
    }

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed with { StatusCode = StatusCodes.Status400BadRequest });

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Reset code: {code}", code);

        await SendResetPasswordEmail(user, code);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(
        ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        IdentityResult identityResult;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            identityResult = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            identityResult = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }

        if (identityResult.Succeeded)
            return Result.Success();

        var error = identityResult.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }

    private static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private async Task SendConfirmationEmail(ApplicationUser user, string code, string? role = null)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        var actionUrl = $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}";

        if (!string.IsNullOrWhiteSpace(role))
            actionUrl += $"&role={Uri.EscapeDataString(role)}";

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FirstName },
                { "{{action_url}}", actionUrl }
            }
        );

        await _emailSender.SendEmailAsync(user.Email!, "✅EDU Bridge : Email Confirmation", emailBody);
    }

    private async Task SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FirstName },
                { "{{action_url}}", $"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
            }
        );

        await _emailSender.SendEmailAsync(user.Email!, "✅EDU Bridge : Change Password", emailBody);
    }
}