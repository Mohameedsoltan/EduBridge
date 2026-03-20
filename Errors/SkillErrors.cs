using EduBridge.Abstractions;

namespace EduBridge.Errors;

public static class SkillErrors
{
    public static readonly Error SkillNotFound = new(
        "Skill.NotFound",
        "The skill with the specified ID was not found or has been deleted.",
        404);

    public static readonly Error DuplicateSkillName = new(
        "Skill.DuplicateName",
        "A skill with the same name already exists.",
        409);
}