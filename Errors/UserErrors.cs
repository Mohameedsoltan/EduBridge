using EduBridge.Abstractions;

namespace EduBridge.Errors;

public static class UserErrors
{
    public static readonly Error UserNotFound = new(
        "User.NotFound", "User not found", StatusCodes.Status404NotFound);

    public static readonly Error DuplicateEmail = new(
        "User.DuplicateEmail", "A user with this email already exists", StatusCodes.Status409Conflict);

    public static readonly Error PasswordMismatch = new(
        "User.PasswordMismatch", "Passwords do not match", StatusCodes.Status400BadRequest);

    public static readonly Error SkillNotFound = new(
        "User.SkillNotFound", "Skill not found for this user", StatusCodes.Status404NotFound);

    public static readonly Error InvalidImage = new(
        "User.InvalidImage", "Invalid image file", StatusCodes.Status400BadRequest);
}