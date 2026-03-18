namespace EduBridge.Entities;

public class Skill : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    // Navigation
    public ICollection<UserSkill> Users { get; set; } = [];
}

public class UserSkill
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
}
