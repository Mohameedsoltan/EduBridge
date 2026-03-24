namespace EduBridge.Contracts.Doctor;

public record RespondDoctorRequestDto(
    bool IsApproved,
    string? ResponseMessage
);