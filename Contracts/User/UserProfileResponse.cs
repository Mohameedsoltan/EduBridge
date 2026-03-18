namespace EduBridge.Contracts.User;

public record UserProfileResponse(
    string FirstName,
    string LastName,
    string? Bio,
    string? Major,
    string? University,
    string? ProfileImageUrl,
    IEnumerable<string> Skills
);