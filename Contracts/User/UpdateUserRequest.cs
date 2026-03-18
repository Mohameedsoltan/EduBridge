namespace EduBridge.Contracts.User;

public record UpdateUserRequest(
    string? FirstName,
    string? LastName,
    string? Bio,
    string? Major,
    string? University
);