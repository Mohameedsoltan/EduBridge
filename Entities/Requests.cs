using EduBridge.Enums;

namespace EduBridge.Entities;

public abstract class BaseRequest : AuditableEntity
{
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public string? Message { get; set; }
    public string? ResponseMessage { get; set; }
    public DateTime? RespondedAt { get; set; }
}

public class JoinRequest : BaseRequest
{
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public string StudentId { get; set; } = string.Empty;
    public ApplicationUser Student { get; set; } = null!;
    public string? RespondedById { get; set; }
    public ApplicationUser? RespondedBy { get; set; }
}

public class TARequest : BaseRequest
{
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public Guid TAId { get; set; }
    public TeachingAssistant TA { get; set; } = null!;
    public Guid? RespondedByTAId { get; set; }
    public TeachingAssistant? RespondedByTA { get; set; }
}

public class DoctorRequest : BaseRequest
{
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public Guid? RespondedByDoctorId { get; set; }
    public Doctor? RespondedByDoctor { get; set; }
}