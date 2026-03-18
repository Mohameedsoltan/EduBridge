namespace EduBridge.Contracts.Role;

public record AssignRoleRequest(
    string UserId,
    string RoleName
);