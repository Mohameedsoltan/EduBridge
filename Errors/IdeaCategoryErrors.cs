using EduBridge.Abstractions;

namespace EduBridge.Errors;

public static class IdeaCategoryErrors
{
    public static readonly Error CategoryNotFound = new(
        "IdeaCategory.NotFound",
        "The category with the specified ID was not found or has been deleted.",
        404);

    public static readonly Error DuplicateCategoryName = new(
        "IdeaCategory.DuplicateName",
        "A category with the same name already exists.",
        409);
}