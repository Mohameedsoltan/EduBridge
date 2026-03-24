namespace EduBridge.Contracts.Role;

public record UpdateRoleRequest(
    string Name,
    bool IsDefault
);