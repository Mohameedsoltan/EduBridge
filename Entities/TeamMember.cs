using EduBridge.Abstractions.Consts;

namespace EduBridge.Entities;

public class TeamMember : AuditableEntity
{
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public MemberRole Role { get; set; } = MemberRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}