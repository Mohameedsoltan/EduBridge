namespace EduBridge.Contracts.Idea;

public record CreateIdeaRequest(
    string Title,
    string Description,
    string? RepositoryUrl,
    Guid CategoryId,
    List<string> Tags
);