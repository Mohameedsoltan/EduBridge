namespace EduBridge.Contracts.User;

public record UserResponse(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string? ProfileImageUrl,
    DateTime CreatedAt
);