namespace EduBridge.Contracts.Authentication;

public record ConfirmEmailRequest(
    string UserId,
    string Token
);