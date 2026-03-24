namespace EduBridge.Contracts.Authentication;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? Bio,
    string? Major,
    string? University,
    string Role,
    string? GitHubUrl,
    string? LinkedInUrl
);