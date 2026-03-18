namespace EduBridge.Contracts.Doctor;
public record UpdateDoctorRequest(
    string Department,
    string? AcademicTitle,
    string? OfficeLocation,
    int MaxTeams,
    int AvailableTeams
);