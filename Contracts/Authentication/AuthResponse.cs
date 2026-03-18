namespace EduBridge.Contracts.Authentication;

public record AuthResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt
);