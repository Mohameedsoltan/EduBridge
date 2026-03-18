namespace EduBridge.Contracts.Idea;

public record UpdateIdeaTagRequest(
    string? Name,
    Guid? CategoryId
);