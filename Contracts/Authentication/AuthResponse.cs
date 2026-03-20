namespace EduBridge.Contracts.Authentication;

public record AuthResponse(
    string Token, 
    int Expiredin,
    string RefreshToken,
    DateTime ExpiresAt
);