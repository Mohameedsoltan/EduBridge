using EduBridge.Enums;

namespace EduBridge.Entities;

public class Team : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string LeaderId { get; set; } = string.Empty;
    public ApplicationUser Leader { get; set; } = null!;
    public int MaxMembers { get; set; }
    public TeamStatus Status { get; set; } = TeamStatus.Open;
    public Guid? IdeaId { get; set; }
    public Idea? Idea { get; set; }
    public Guid? TaId { get; set; }
    public TeachingAssistant? Ta { get; set; }

    // Navigation
    public ICollection<TeamMember> Members { get; set; } = [];
    public ICollection<JoinRequest> JoinRequests { get; set; } = [];
    public ICollection<TARequest> TARequests { get; set; } = [];
    public Guid? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
}

public class TeamMember : AuditableEntity
{
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public MemberRole Role { get; set; } = MemberRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
