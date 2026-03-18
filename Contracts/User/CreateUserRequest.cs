namespace EduBridge.Contracts.User;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? Bio,
    string? Major,
    string? University,
    string Role
);