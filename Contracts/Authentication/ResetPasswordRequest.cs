namespace EduBridge.Contracts.Authentication;

public record ResetPasswordRequest(
    string Email,
    string Code,
    string NewPassword,
    string ConfirmNewPassword
);