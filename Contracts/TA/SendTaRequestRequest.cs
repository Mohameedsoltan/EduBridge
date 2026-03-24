namespace EduBridge.Contracts.TA;

public record SendTaRequestRequest(
    Guid TAId,
    string? Message
);